import { StdioServerTransport } from '@modelcontextprotocol/sdk/server/stdio.js';
import { createOpenFairyGuiMcpServer } from '@openfairygui/mcp';
import { z } from 'zod';
import { PIPELINE_GUIDANCE } from './guidance.mjs';
import { paths } from './paths.mjs';
import {
  formatInspectReport,
  formatValidateReport,
  inspectProject,
  publishProject,
  validateProject,
} from './project.mjs';

const emptyInput = z.object({});

function textResult(payload, isError = false) {
  const text = typeof payload === 'string' ? payload : JSON.stringify(payload, null, 2);
  return {
    content: [{ type: 'text', text }],
    isError,
  };
}

function registerHowePipeline(server) {
  server.registerTool(
    'howe_fgui_inspect_project',
    {
      title: 'Inspect HoweFramework FairyGUI Project',
      description: 'Inspect FGUIProject/FGUIProject.fairy. Does not write files.',
      inputSchema: emptyInput,
      annotations: { readOnlyHint: true, idempotentHint: true, openWorldHint: false },
    },
    async () => {
      try {
        const result = await inspectProject();
        return textResult({
          ok: true,
          summary: formatInspectReport(result),
          ...result,
        });
      } catch (error) {
        return textResult({
          ok: false,
          error: error instanceof Error ? error.message : String(error),
        }, true);
      }
    },
  );

  server.registerTool(
    'howe_fgui_validate_project',
    {
      title: 'Validate HoweFramework FairyGUI Project',
      description: 'Validate FGUIProject structure, references, and source files without writing.',
      inputSchema: emptyInput,
      annotations: { readOnlyHint: true, idempotentHint: true, openWorldHint: false },
    },
    async () => {
      try {
        const result = await validateProject();
        const invalid = result.report.status === 'invalid';
        return textResult({
          ok: !invalid,
          summary: formatValidateReport(result),
          ...result,
        }, invalid);
      } catch (error) {
        return textResult({
          ok: false,
          error: error instanceof Error ? error.message : String(error),
        }, true);
      }
    },
  );

  server.registerTool(
    'howe_fgui_publish_project',
    {
      title: 'Publish HoweFramework FairyGUI Project',
      description: 'Publish FGUIProject to Client/Assets/GameMain/UI as Unity *_fui.bytes. Does not generate C# bindings.',
      inputSchema: emptyInput,
      annotations: { readOnlyHint: false, destructiveHint: true, idempotentHint: false, openWorldHint: false },
    },
    async () => {
      try {
        const result = await publishProject();
        return textResult({
          ok: true,
          nextStep: 'Open Unity (or use Game Framework/FairyGUI/Generate Code) so AssetPostprocessor can emit UIFormId and GameMain logic stubs.',
          ...result,
        });
      } catch (error) {
        return textResult({
          ok: false,
          error: error instanceof Error ? error.message : String(error),
        }, true);
      }
    },
  );

  server.registerPrompt(
    'howe_fgui_pipeline',
    {
      title: 'HoweFramework FairyGUI Pipeline',
      description: 'HoweFramework naming, FGUIProject session path, publish output, and Unity binding steps.',
    },
    () => ({
      messages: [
        {
          role: 'user',
          content: { type: 'text', text: PIPELINE_GUIDANCE },
        },
      ],
    }),
  );
}

export function createHoweFguiMcpServer() {
  const server = createOpenFairyGuiMcpServer({
    name: 'howe-fgui',
    allowedProjectRoots: [paths.fguiProjectDir],
  });
  registerHowePipeline(server);
  return server;
}

export async function startMcpServer() {
  const server = createHoweFguiMcpServer();
  await server.connect(new StdioServerTransport());
}
