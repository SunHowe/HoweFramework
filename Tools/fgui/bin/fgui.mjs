#!/usr/bin/env node

import {
  formatInspectReport,
  formatValidateReport,
  inspectProject,
  publishProject,
  validateExitCode,
  validateProject,
} from '../src/project.mjs';
import { startMcpServer } from '../src/mcp-server.mjs';
import { PIPELINE_GUIDANCE } from '../src/guidance.mjs';
import { paths } from '../src/paths.mjs';

const USAGE = `HoweFramework FairyGUI CLI (OpenFairyGUI wrapper)

Usage:
  node Tools/fgui/bin/fgui.mjs <command> [--json]

Commands:
  inspect   Inspect FGUIProject
  validate  Validate FGUIProject
  publish   Publish to Client/Assets/GameMain/UI
  mcp       Start the composed MCP server (stdio)
  guidance  Print HoweFramework UI pipeline notes

Fixed paths:
  project  ${paths.fairyPath}
  output   ${paths.publishOutputDir}
`;

async function main() {
  const args = process.argv.slice(2);
  const command = args.find((arg) => !arg.startsWith('-')) ?? 'help';
  const json = args.includes('--json');

  switch (command) {
    case 'inspect': {
      const result = await inspectProject();
      if (json) {
        process.stdout.write(`${JSON.stringify(result, null, 2)}\n`);
      } else {
        process.stdout.write(`${formatInspectReport(result)}\n`);
      }
      return;
    }
    case 'validate': {
      const result = await validateProject();
      if (json) {
        process.stdout.write(`${JSON.stringify(result, null, 2)}\n`);
      } else {
        process.stdout.write(`${formatValidateReport(result)}\n`);
      }
      process.exitCode = validateExitCode(result.report.status);
      return;
    }
    case 'publish': {
      process.stderr.write(`Publishing ${paths.fairyPath}\n`);
      const result = await publishProject();
      if (json) {
        process.stdout.write(`${JSON.stringify(result, null, 2)}\n`);
      } else {
        process.stdout.write(`Published to ${result.outputDir}\n`);
        if (result.packages.length === 0) {
          process.stdout.write('No *_fui.bytes packages in output (project may have no packages yet).\n');
        } else {
          for (const pkg of result.packages) {
            process.stdout.write(`  ${pkg.fileName}\n`);
          }
        }
      }
      return;
    }
    case 'mcp':
      await startMcpServer();
      return;
    case 'guidance':
      process.stdout.write(`${PIPELINE_GUIDANCE}\n`);
      return;
    case 'help':
    case '-h':
    case '--help':
      process.stdout.write(USAGE);
      return;
    default:
      process.stderr.write(`Unknown command: ${command}\n\n${USAGE}`);
      process.exitCode = 1;
  }
}

main().catch((error) => {
  process.stderr.write(`${error instanceof Error ? error.stack ?? error.message : String(error)}\n`);
  process.exitCode = 1;
});
