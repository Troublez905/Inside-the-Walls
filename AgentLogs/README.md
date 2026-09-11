# Inside the Walls Agent Collaboration Log

This directory records the useful, reviewable collaboration produced by project agents.
It contains task assignments, findings, decisions, critique, responses, verification, and
final reports. It does not contain private chain-of-thought or hidden model reasoning.

## File convention

Each agent owns one Markdown log named:

`YYYY-MM-DD--agent-role--task.md`

Cross-agent feedback is copied into both the reviewer's log and the receiving agent's
log under a `Feedback` heading. The lead records integration decisions in
`YYYY-MM-DD--lead-producer--integration.md`.

## Required entries

- Timestamp and status
- Assignment and allowed edit area
- Evidence inspected
- Decisions and rationale
- Feedback sent or received
- Files changed
- Verification results
- Limitations and next handoff

Use `append-agent-log.ps1` to append a timestamped entry without replacing existing
history.
