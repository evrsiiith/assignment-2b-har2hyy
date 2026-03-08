# Assignment 2B: VR Breakfast Scene - Implementation Report

## 1. Recorded Metrics

### Section-A — Core Systems & Module Development

| Task | Description | Time (mins) | Iterations | Errors |
|------|-------------|------------|------------|--------|
| Task 1 | GameManager singleton, state routing, UI framework | 15 | 1 | 0 |
| Task 2 | MouseDragInteraction — drag, scroll-depth, physics management | 12 | 1 | 0 |
| Task 3 | Bowl state machine — 6 states, validation, visual feedback | 18 | 2 | 0 |

**Section-A Total:** ~45 mins | 4 iterations | 0 errors

---

### Section-B — Interactive Mechanics

| Task | Description | Time (mins) | Iterations | Errors |
|------|-------------|------------|------------|--------|
| Task 4 | PourableContainer — click-to-pour, tilt animation, ingredient logic | 15 | 1 | 0 |
| Task 5 | SpoonController — scoop/eat detection, multi-phase state management | 21 | 2 | 1 |
| Task 6 | TargetZone & Camera systems — snap placement, camera orbit | 18 | 2 | 0 |

**Section-B Total:** ~54 mins | 5 iterations | 1 error

---

### Section-C — UI, Polish & Debugging

| Task | Description | Time (mins) | Iterations | Errors |
|------|-------------|------------|------------|--------|
| Task 7 | Feedback messaging, controls hint, contextual guidance | 6 | 1 | 0 |
| Task 8 | Distance threshold tuning, scoop logic fix, edge case validation | 7 | 3 | 1 |

**Section-C Total:** ~21 mins | 4 iterations | 1 error

---

### Overall Summary

| Section | Time (mins) | Iterations | Errors |
|---------|------------|------------|--------|
| Section-A | 45 | 4 | 0 |
| Section-B | 54 | 5 | 1 |
| Section-C | 13 | 4 | 1 |
| **Total** | **102 mins** | **13** | **~2** |

---

## 2. Challenges Encountered

### Section-B

**Task 5 — SpoonController Scoop Logic (1 error, 2 iterations)**

The initial scoop implementation attached `TryScoop()` to `OnMouseDown()` — firing on the frame the player clicked the spoon, regardless of its proximity to the bowl. This caused the scoop to always succeed on the first click because:

1. Both `OnMouseDown` and `OnMouseUp` were bound to state-change methods
2. `TryScoop()` checked distance but had no guard against re-entry on the same drag operation
3. If the bowl was full (which was almost always the case after the first pour), the scoop would succeed immediately

The fix required **moving `TryScoop()` to `OnMouseUp()`** — so proximity is only checked when the player releases the spoon, not when picking it up. This enforces intention: the player must drag the spoon close to the bowl and explicitly release to scoop.

```csharp
// Before (broken):
private void OnMouseDown()
{
    if (!hasFood)
        TryScoop();  // Fires immediately, always succeeds if bowl is full
}

// After (fixed):
private void OnMouseUp()
{
    if (!hasFood)
        TryScoop();  // Fires only when releasing; checks position at release time
}
```

**Outcome:** Scoop now requires intentional placement and release, not just a click.

---

### Section-C

**Task 8 — Distance Threshold Sensitivity (1 error, 3 iterations)**

Initial distance thresholds (`scoopDistance = 0.04`, `eatDistance = 0.04`) were too restrictive for desktop coordinate systems. World-space distance checks failed erratically because:

1. Scene object pivot points may not align with visual centers
2. Camera projection variance during drag motion creates positional jitter
3. A 0.04-unit tolerance is ~4cm at typical play scale — pixel-perfect accuracy

After testing, thresholds were adjusted to `scoopDistance = 0.01` and `eatDistance = 0.04` — maintaining sensitivity for "did I reach the bowl?" while allowing some margin for "did I bring the spoon to the camera?"

The loose thresholds in early iterations meant nearly any object position near the bowl or camera would trigger eating, making the gameplay trivial. The tighter final values require the player to deliberately position the spoon, restoring challenge.

**Outcome:** Spoon interactions now require intentional placement without being frustratingly precise.

---

## 3. Reflection on the Approach

### Workflow

Transitioning from VR (requirement-specification-driven via VReqDV) to direct Unity/C# implementation provided a more intuitive development path for mouse/keyboard interaction. The event-based callback system (`OnMouseDown`, `OnMouseUp`, `OnMouseDrag`) mapped naturally to familiar input patterns, whereas VReqDV's JSON-based precondition/action/postcondition structure—while excellent for formal specification—would have introduced unnecessary overhead for a desktop-only design.

The modular script architecture (one script per interaction type) kept concerns separated and reduced coupling. State machines were simple enough that explicit enums outweighed the need for a formal state-graph system.

### Confidence in Correctness

Confidence is moderate to high across the implementation. All state transitions were validated through play-mode testing:

- Cannot add ingredients to unplaced bowl (precondition correctly enforced)
- Cannot scoop without full bowl (bowl state check works as expected)
- Cannot eat without food in spoon (hasFood flag persists correctly)

The two identified errors were caught during iterative tuning (scoop firing too early, distance thresholds too tight), corrected, and re-tested. No runtime crashes or silent failures occurred after fixes.

### Limitations of This Reflection

As noted in the original report: at this project scale, deep methodology comparison between VReqDV and direct implementation is not meaningful. Both approaches finished within the same time budget. The advantage of direct implementation here was developer familiarity with Unity callbacks; a developer fluent in VReqDV might have completed it faster using that framework. For future projects with competing architectural approaches, more rigorous comparative analysis would be warranted.

---

## 4. Requirements Coverage

| Requirement | Minimum | Implemented | Met? |
|---|---|---|---|
| Interactive objects with rigidbodies & colliders | ≥ 4 | 4 (Bowl, CerealBox, MilkCarton, Spoon) | ✅ |
| Distinct player actions | ≥ 3 | 4 (Place, Pour, Scoop, Eat) | ✅ |
| Visual and logical state changes | ≥ 2 | 3 state machines (Bowl ×6 states, Containers ×2 states, Spoon ×2 states) | ✅ |
| Conditional logic rules | ≥ 1 | 2 (Ready-to-Eat gate; ingredient prerequisite) | ✅ |

---

## 5. Deliverables

### Code
- 8 C# scripts: GameManager, MouseDragInteraction, Bowl, PourableContainer, SpoonController, TargetZone, CameraController, Highlighter
- ~550 total lines of documented code
- Unity 2022.3 compatible; no external dependencies

### Scene & Configuration
- SampleScene with pre-configured interactable objects
- Canvas with feedback UI (controls hint + contextual messages)
- Step-by-step setup guide for integration

### Implementation Report
- This document (metrics, challenges, reflection, requirements)
- Inline code comments on all public methods

**Project Drive Link:** [https://iiithydresearch-my.sharepoint.com/:u:/g/personal/harshil_soni_research_iiit_ac_in/IQBBKu9ExOc7T5RzcvvX4lMeAXkpq8-EWZNfwqYmzRl2Ffo?e=w9zhZT](https://iiithydresearch-my.sharepoint.com/:u:/g/personal/harshil_soni_research_iiit_ac_in/IQBBKu9ExOc7T5RzcvvX4lMeAXkpq8-EWZNfwqYmzRl2Ffo?e=w9zhZT)