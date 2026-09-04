import fs from 'node:fs';
import path from 'node:path';
import { NodeIO } from '@openfairygui/core/node';
import { inspect, resolvePublishOptions } from '@openfairygui/functions';
import { publishNode, validateProjectNode } from '@openfairygui/functions/node';
import { assertFairyProject, paths } from './paths.mjs';

const UNITY_PROJECT_TYPE = 0;

export async function inspectProject() {
  const { fairyPath } = assertFairyProject();
  const io = new NodeIO();
  const document = await io.readProject(fairyPath);
  return {
    fairyPath,
    report: inspect(document),
  };
}

export async function validateProject() {
  const { fairyPath } = assertFairyProject();
  const report = await validateProjectNode(fairyPath);
  return {
    fairyPath,
    report,
  };
}

export async function publishProject() {
  const { fairyPath, fguiProjectDir, assetsPath, publishOutputDir } = assertFairyProject();
  const io = new NodeIO();
  const document = await io.readProject(fairyPath);
  document.getRoot().setProjectType(UNITY_PROJECT_TYPE);

  const resolved = resolvePublishOptions(document, {
    targetProjectType: UNITY_PROJECT_TYPE,
  });

  await publishNode({
    document,
    output: publishOutputDir,
    compressed: resolved.compressed,
    fileExtension: resolved.fileExtension,
    packages: resolved.packages,
    assetsPath,
    atlas: resolved.atlas,
  });

  const packages = listPublishedPackages(publishOutputDir);
  return {
    fairyPath,
    projectDir: fguiProjectDir,
    outputDir: publishOutputDir,
    fileExtension: resolved.fileExtension,
    compressed: resolved.compressed,
    packages,
  };
}

function listPublishedPackages(outputDir) {
  if (!fs.existsSync(outputDir)) {
    return [];
  }

  return fs.readdirSync(outputDir)
    .filter((name) => name.endsWith('_fui.bytes'))
    .map((name) => ({
      fileName: name,
      packageName: name.slice(0, -'_fui.bytes'.length),
      path: path.join(outputDir, name),
    }))
    .sort((a, b) => a.packageName.localeCompare(b.packageName));
}

export function formatInspectReport({ fairyPath, report }) {
  const lines = [
    `Project: ${fairyPath}`,
    `ID: ${report.projectId}`,
    `Type: ${report.projectType}, Version: ${report.version}`,
    '',
    `Packages: ${report.totals.packages}`,
    `  Images: ${report.totals.images}`,
    `  Sounds: ${report.totals.sounds}`,
    `  Fonts: ${report.totals.fonts}`,
    `  MovieClips: ${report.totals.movieClips}`,
    `  Components: ${report.totals.components}`,
    `  DisplayObjs: ${report.totals.displayObjects}`,
    `  Gears: ${report.totals.gears}`,
    `  Controllers: ${report.totals.controllers}`,
    `  Transitions: ${report.totals.transitions}`,
    '',
    'Package details:',
  ];

  if (report.packages.length === 0) {
    lines.push('  (none)');
    return lines.join('\n');
  }

  for (const pkg of report.packages) {
    const res = pkg.resources;
    lines.push(
      `  ${pkg.name} (${pkg.id}): ${res.images.count} img, ${res.sounds.count} snd, ${res.fonts.count} font, ${res.components.count} comp`,
    );
  }

  return lines.join('\n');
}

export function formatValidateReport({ fairyPath, report }) {
  const lines = [`${report.status.toUpperCase()}: ${fairyPath}`];
  for (const diagnostic of report.diagnostics ?? []) {
    const source = diagnostic.sourcePath ? ` (${diagnostic.sourcePath})` : '';
    lines.push(
      `${String(diagnostic.severity).toUpperCase()} ${diagnostic.code} ${diagnostic.path}${source}: ${diagnostic.message}`,
    );
  }
  return lines.join('\n');
}

export function validateExitCode(status) {
  if (status === 'valid') {
    return 0;
  }
  if (status === 'invalid') {
    return 1;
  }
  return 2;
}
