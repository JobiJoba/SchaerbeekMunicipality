# Remove Document

Removes an attached document from a registration case and deletes the stored file.

This is the document **correction** slice in the [intake correction pattern](./README.md#intake-corrections-phase-21). Wrong scans are fixed by removing the file and [attaching](./attach-document.md) a replacement — there is no in-place replace in Phase 2.1.

## Overview

| | |
|---|---|
| **Handler** | `RemoveDocumentHandler` |
| **Endpoint** | `RemoveDocumentEndpoint` |
| **Route** | `DELETE /api/registration/cases/{id}/documents/{documentId}` |
| **Blazor entry** | `RegistrationCaseDetail.razor` (per-row remove action with `AppConfirmDialog`) |
| **Response** | `RemoveDocumentResponse(CaseId, DocumentId, LegalResidenceEstablished, PolicyMessage)` |

## Domain logic

Uses `RegistrationCase.EnsureCanAttachDocuments()`, which delegates to `EnsureIntakeDataEditable()` — allowed in `Intake` and `UnderReview` only.

The handler:

1. Loads the case and document; verifies the document belongs to the case
2. Removes the database row via `IAdministrativeDocumentRepository.Remove()`
3. Deletes the file from `IDocumentStorage`
4. Re-runs `RegistrationResidenceEvaluator` (document types may affect policy in later phases)
5. Persists via `IRegistrationCaseRepository.SaveChangesAsync()`

## UI

Each document row shows a remove icon when the case is editable. Removal requires confirmation via `AppConfirmDialog` (destructive action). After success, `ReloadCase()` refreshes the list and checklist.

## Error responses

| Status | Condition | Blazor handling |
|--------|-----------|-----------------|
| `404` | Case or document not found | Snackbar + reload |
| `409` | Case not editable, or document belongs to another case | Snackbar with domain message |
| `200` | Success | Snackbar + page reload |

## Audit

Structured log at handler level: case id, document id, timestamp (`ILogger`).

## Related

- [Attach document](./attach-document.md) — first attach and replacement upload
- [Phase 2.1 — Intake corrections](../../phases/phase-2.1-intake-corrections.md)
