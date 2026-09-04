import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const toolsFguiDir = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
const repoRoot = path.resolve(toolsFguiDir, '../..');

export const paths = Object.freeze({
  toolsFguiDir,
  repoRoot,
  fguiProjectDir: path.join(repoRoot, 'FGUIProject'),
  fairyPath: path.join(repoRoot, 'FGUIProject', 'FGUIProject.fairy'),
  assetsPath: path.join(repoRoot, 'FGUIProject', 'assets'),
  publishOutputDir: path.join(repoRoot, 'Client', 'Assets', 'GameMain', 'UI'),
});

export function assertFairyProject() {
  if (!fs.existsSync(paths.fairyPath)) {
    throw new Error(`FairyGUI project not found: ${paths.fairyPath}`);
  }
  return paths;
}
