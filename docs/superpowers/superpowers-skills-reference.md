# Superpowers Skills Reference

Short reference for the `superpowers` skills currently available in this environment.

## How To Read This

- **What it does**: the short purpose of the skill.
- **Trigger**: the exact kind of situation where it should be invoked.
- **Small example**: one quick example of that trigger.

---

## `using-superpowers`

**What it does:** Checks whether any other `superpowers` skill applies before doing work.

**Trigger:** Invoke at the start of a conversation, before answering or taking action, when there is any chance another skill may apply.

**Small example:**  
User says, "Add role-based redirects after login." Before asking clarifying questions or editing files, first check which `superpowers` workflow skill applies.

---

## `brainstorming`

**What it does:** Turns an idea into an approved design before implementation starts.

**Trigger:** Invoke before creative work such as adding a feature, building a component, or changing behavior.

**Small example:**  
User says, "Build a patient timeline screen." First explore context, ask one clarifying question at a time, propose approaches, and get design approval before coding.

---

## `writing-plans`

**What it does:** Writes a detailed implementation plan with exact files, steps, tests, and commits.

**Trigger:** Invoke when an approved spec or clear multi-step requirements exist and implementation is about to begin.

**Small example:**  
The design for a new workspace module is approved. Next, write a plan that lists the files to create, the tests to add first, and the order of work.

---

## `using-git-worktrees`

**What it does:** Sets up an isolated git worktree so feature work does not disturb the current workspace.

**Trigger:** Invoke before executing a plan or starting feature work that should happen in an isolated branch/workspace.

**Small example:**  
Before implementing a new appointments flow, create a dedicated worktree like `.worktrees/appointments-flow` and verify the baseline tests are clean there.

---

## `subagent-driven-development`

**What it does:** Executes a written plan task-by-task using subagents, with review loops after each task.

**Trigger:** Invoke when there is already a written implementation plan, tasks are mostly independent, and execution will stay in the current session.

**Small example:**  
A plan has five tasks. For Task 1, dispatch an implementer subagent, then run spec review, then code quality review, then move to Task 2.

---

## `executing-plans`

**What it does:** Executes a written implementation plan step-by-step in a structured workflow.

**Trigger:** Invoke when there is already a written plan and that plan should be executed directly rather than through the subagent-driven flow.

**Small example:**  
There is a saved plan for a reporting feature. Load it, review it for gaps, create todos from the tasks, and work through the steps in order.

---

## `test-driven-development`

**What it does:** Enforces test-first development: write the failing test, watch it fail, then write the minimal code to pass.

**Trigger:** Invoke before writing implementation code for a feature, bug fix, refactor, or behavior change.

**Small example:**  
Before fixing "empty phone number saves successfully," first write a test that expects validation to fail, run it to confirm it fails, then implement the validation.

---

## `systematic-debugging`

**What it does:** Forces root-cause investigation before any fix is proposed.

**Trigger:** Invoke for any bug, failing test, build break, integration problem, or unexpected behavior.

**Small example:**  
The login test fails after a routing change. Instead of patching guards immediately, reproduce the issue, inspect the error, trace the flow, compare with a working route, and identify the root cause first.

---

## `dispatching-parallel-agents`

**What it does:** Splits independent investigations across multiple agents so they can run in parallel.

**Trigger:** Invoke when there are two or more independent failures or tasks that do not share state and can be investigated separately.

**Small example:**  
Three test files fail for unrelated reasons: one auth issue, one queue issue, and one export issue. Dispatch one agent per file instead of debugging them sequentially.

---

## `requesting-code-review`

**What it does:** Requests a focused review after meaningful implementation work so issues are caught early.

**Trigger:** Invoke after a major task, after a major feature, or before merging.

**Small example:**  
After finishing the clinical workspace feature, request code review against the task requirements and the diff before moving toward merge.

---

## `receiving-code-review`

**What it does:** Handles review feedback rigorously by verifying suggestions before implementing them.

**Trigger:** Invoke when code review comments arrive, especially when they are unclear, broad, or potentially incorrect for the codebase.

**Small example:**  
A reviewer says, "Remove this fallback and simplify the guard." First verify whether that fallback protects a real case in the app before changing it.

---

## `verification-before-completion`

**What it does:** Requires fresh verification evidence before claiming something is complete, fixed, or passing.

**Trigger:** Invoke before saying tests pass, before saying a bug is fixed, before marking a task complete, and before commit or PR creation.

**Small example:**  
Before saying "the build is fixed," run the actual build command again, read the result, and only then report the build status.

---

## `finishing-a-development-branch`

**What it does:** Handles the end-of-workflow decision after implementation is done and tests are passing.

**Trigger:** Invoke when work is complete and the next question is how to integrate it: merge, PR, keep branch, or discard.

**Small example:**  
All tasks are complete and tests pass. Present the four structured choices: merge locally, push and create a PR, keep the branch as-is, or discard the work.

---

## `writing-skills`

**What it does:** Guides creation or editing of skills using a TDD-style process for documentation and workflow rules.

**Trigger:** Invoke when creating a new skill, editing an existing skill, or validating a skill before deployment.

**Small example:**  
You want to create a skill for handling flaky polling tests. First capture how an agent currently fails without the skill, then write the skill, then test whether the agent now follows it.

---

## Quick Workflow Map

### New feature

`using-superpowers` -> `brainstorming` -> `writing-plans` -> `using-git-worktrees` -> `subagent-driven-development` or `executing-plans` -> `test-driven-development` -> `requesting-code-review` -> `verification-before-completion` -> `finishing-a-development-branch`

### Bug fix

`using-superpowers` -> `systematic-debugging` -> `test-driven-development` -> `requesting-code-review` -> `verification-before-completion`

### Review feedback

`using-superpowers` -> `receiving-code-review` -> `verification-before-completion`

### Writing or editing skills

`using-superpowers` -> `writing-skills`
