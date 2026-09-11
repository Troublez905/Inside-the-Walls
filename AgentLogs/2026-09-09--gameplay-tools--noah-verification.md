# Noah Verification Harness

## Conversation

- Lead Producer -> Gameplay Tools: Implement an editor-compatible Noah smoke harness. Edit only NoahAnimationVerification.cs and this log; inspect the existing player, frontend, and installed URP source. Do not call Unity, change the player/builder/frontend, install packages, or create reviewer/child agents. The prior hidden-player capture was black and measured zero leg motion. Preserve the frontend's no-save workflow.
- Gameplay Tools -> Lead Producer: Confirmed URP 17.4 supports RenderPipeline.StandardRequest. The harness will capture through that engine API without an assembly-definition dependency, expose polling properties, sample motion repeatedly, and preserve Animator culling settings while reporting visibility/state/speed diagnostics.
- Lead Producer -> Gameplay Tools: Unity MCP is connected and the editor is idle. The lead is adding a public development-only frontend entry point and an editor menu launcher that disable saves before starting the prototype. Notify the lead when the harness is ready for compilation and a live run.
- Gameplay Tools -> Lead Producer: Implemented explicit camera captures, black-frame detection, twelve motion samples, public completion/results, and terminal cleanup. Editor completion leaves play mode active with movement suppressed; standalone completion retains exit codes. Live Unity compilation and rendering remain assigned to the lead.

## Changes And Decisions

- Capture uses RenderPipeline.SupportsRenderRequest and SubmitRenderRequest with a 1280x720 RenderTexture. The installed URP Runtime/UniversalRenderPipeline.cs and Tests/Runtime/RuntimeTests.cs establish support. The render target is cleared before rendering and all render resources are restored/released in finally.
- Captures report lit-pixel fraction and mean luminance and fail if fewer than 0.1 percent of pixels exceed a near-black threshold. Camera captures include scene rendering; the IMGUI HUD is excluded. Failed captures remain available as evidence.
- Walking checks both lower legs over twelve staggered samples and compares every sample against earlier samples to avoid mistaking a repeated cycle pose for absent movement. No forced Animator update, culling mode change, or extra render occurs inside the sampling loop.
- Diagnostics report Animator culling, initialization, enabled/active state, speed, state hash/name checks, normalized time, transitions, renderer visibility/bounds, and main-camera frustum/layer/viewport information. Visibility reported by Renderer.isVisible can include the editor Scene view; the separate main-camera checks make that distinction explicit.
- Public IsRunning, IsComplete, Succeeded, CurrentStep, Result, FailureReason, OutputDirectory, and Diagnostics properties support editor polling. A RUNNING result replaces stale terminal results at startup. Terminal completion stops coroutines, suppresses movement, and removes the temporary collision blocker.
- Only standalone development players quit automatically. Editor runs remain in play mode, including failures; the final front-detail camera remains visible after success. Existing frontend verification-session save suppression remains the entry contract.

## Verification And Limits

- Source/API inspection performed against the installed Unity 6000.4 / URP 17.4 package; Unity runtime compilation and rendered validation are owned by the lead.
- No Unity launch or MCP call, package/assembly-definition edit, reviewer agent, child agent, Git commit, or push performed.
- Black detection checks render validity, not whether Noah is framed well or aesthetically correct. The camera evidence still requires visual inspection.
- Camera requests can render a previously hidden renderer, but the motion sample loop preserves production culling and reports its observable state. A culling-related failure must be fixed in production by the lead, not bypassed in this harness.
- Implementation complete; awaiting lead integration and live verification.
