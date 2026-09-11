# Unity MCP setup

## Prepared on 2026-09-08

- Added `com.coplaydev.unity-mcp` pinned to the official `v10.0.0` Git tag in the project manifest. Unity has recorded it in packages-lock.json.
- Confirmed uv 0.11.6 and Python 3.11.9 are available.
- Downloaded the matching Python server with `uvx --from mcpforunityserver==10.0.0 mcp-for-unity --help`; the CLI completed successfully.
- Codex already has `[mcp_servers.unityMCP]` pointing to `http://127.0.0.1:8080/mcp`. Existing approval settings were preserved.
- The project is now open in Unity 6000.4.0f1. Its package upgrades were preserved; the earlier 6000.3 batch editor is not running.

## Remaining connection step

The tool environment blocked automatic background-server launch. No successful editor/MCP handshake has been verified yet.

1. Bring Unity to the foreground and let Package Manager/import/compilation finish. If necessary, use Assets > Refresh.
2. Open Window > MCP for Unity. Complete its dependency wizard if shown; uv and Python are already installed.
3. Select HTTP local transport at `http://127.0.0.1:8080`, start the server, and start/connect the Unity session. Look for Connected.
4. Return to Codex for an editor-state and console check. If this Codex session does not discover the newly running server, reconnect the MCP integration before testing.

Keep the server bound to loopback. No LAN exposure, firewall changes, cloud credentials, or login-startup service were configured.

These steps follow the project's official [installation guide](https://coplaydev.github.io/unity-mcp/getting-started/install) and [transport guide](https://coplaydev.github.io/unity-mcp/architecture/transports). The Unity integration skill was used to require a live state/console check before declaring the connection complete.
