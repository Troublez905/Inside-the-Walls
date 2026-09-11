// Local-only MCP client for sessions whose native tool discovery predates Unity startup.
const endpoint = 'http://127.0.0.1:8080/mcp';
const [operation, name, argument = '{}'] = process.argv.slice(2);
let session;
let requestId = 0;

async function request(method, params, notification = false) {
  const headers = { 'Content-Type': 'application/json', Accept: 'application/json, text/event-stream' };
  if (session) headers['Mcp-Session-Id'] = session;
  const response = await fetch(endpoint, {
    method: 'POST', headers,
    body: JSON.stringify({ jsonrpc: '2.0', ...(notification ? {} : { id: ++requestId }), method, params }),
    signal: AbortSignal.timeout(45000)
  });
  session = response.headers.get('mcp-session-id') || session;
  const raw = await response.text();
  if (!response.ok) throw new Error(`MCP HTTP ${response.status}: ${raw.slice(0,500)}`);
  if (!raw) return null;
  const events = raw.split('\n').filter(line => line.startsWith('data:'));
  const message = JSON.parse(events.length ? events.at(-1).slice(5) : raw);
  if (message.error) throw new Error(JSON.stringify(message.error));
  return message.result;
}

function unpack(result) {
  if (result?.structuredContent) return result.structuredContent;
  const contents = result?.contents || result?.content;
  if (!contents) return result;
  return contents.filter(item => typeof item.text === 'string').map(item => {
    try { return JSON.parse(item.text); } catch { return item.text; }
  });
}

try {
  await request('initialize', { protocolVersion: '2024-11-05', capabilities: {}, clientInfo: { name: 'InsideTheWalls-local-tools', version: '1.0' } });
  await request('notifications/initialized', {}, true);
  let result;
  if (operation === 'read') result = unpack(await request('resources/read', { uri: name }));
  else if (operation === 'schema') {
    const tools = (await request('tools/list', {})).tools;
    result = tools.filter(tool => name.split(',').includes(tool.name)).map(tool => ({ name: tool.name, description: tool.description, inputSchema: tool.inputSchema }));
  } else if (operation === 'tool') result = unpack(await request('tools/call', { name, arguments: JSON.parse(argument) }));
  else throw new Error('Usage: client.mjs read <uri> | schema <tool,...> | tool <name> <json>');
  console.log(JSON.stringify(result));
} finally {
  if (session) await fetch(endpoint, { method: 'DELETE', headers: { 'Mcp-Session-Id': session }, signal: AbortSignal.timeout(3000) });
}
