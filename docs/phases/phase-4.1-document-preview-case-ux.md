# Phase 4.1 — Document preview & case UX

- **Status:** Complete
- **Completed:** July 2026
- **Goal:** Let officers preview and download attached documents without leaving the case, and surface intake progress at a glance on the case list.

---

## Summary

Phase 4 delivered address, household, and civil status intake. Officers still had to work with documents as a flat filename list at the bottom of a long case page — no inline preview, no download link, and the case list only showed whether identity was established.

Phase 4.1 is a **cross-cutting UX phase** between Phase 4 and Phase 5: it adds read access to stored files, a sticky document sidebar on the case detail page, and a compact checklist progress indicator on the case list. No domain model changes.

---

## Problem statement

| Area | Before | Officer impact |
|------|--------|----------------|
| **Document access** | Upload + remove only; files on disk unreachable from UI | Cannot verify a scan without leaving the app or digging on disk |
| **Case detail layout** | Document upload buried below all intake steps | Long scroll to attach or review evidence while working residence/address |
| **Case list** | Single identity icon in the Progress column | Cannot see legal residence or address progress without opening each case |

---

## Deliverables checklist

| Deliverable | Status | Notes |
|-------------|--------|-------|
| `IDocumentStorage.OpenReadAsync` | Done | Returns a read stream; `FileNotFoundException` when missing |
| `DownloadDocument` slice | Done | Handler + `GET …/documents/{documentId}` endpoint |
| `RegistrationCaseDocumentPanel` | Done | Upload, list, inline preview, fullscreen dialog, download link |
| `DocumentPreviewContent` | Done | PDF iframe + image render; fallback for other types |
| `DocumentPreviewDialog` | Done | Full-screen preview via `IDialogService` |
| Case detail two-column layout | Done | Intake steps left (`lg=7`), sticky document panel right (`lg=5`) |
| `RegistrationCaseChecklistProgress` | Done | `n/3` + icons for identity, legal residence, address |
| `ListRegistrationCases` extended DTO | Done | Adds `LegalResidenceEstablished`, `AddressDeclared` |
| Preview CSS (`app.css`) | Done | Sticky panel, selected row, preview frame sizing |

---

## API routes

| Method | Route | Slice |
|--------|-------|-------|
| `GET` | `/api/registration/cases/{id}/documents/{documentId}` | Download / stream document for preview |

Streams the file with the correct `Content-Type` (`application/pdf`, `image/jpeg`, `image/png`, or `application/octet-stream`). Range requests enabled for PDF seeking in the browser.

---

## UI behaviour

### Case detail — document panel

- Case detail uses `MaxWidth.False` and a `MudGrid`: intake steps on the left, documents on the right.
- On viewports ≥ 1280px the document panel is **sticky** (`top: 80px`) so evidence stays visible while scrolling intake forms.
- Click a document row to preview inline; selected row is highlighted.
- Toolbar actions: **fullscreen** (dialog) and **download** (opens stream in new tab).
- PDFs render in an `<iframe>`; images in an `<img>`; other types show an empty state with download only.

### Case list — progress column

- The **Progress** column replaces the single identity icon.
- Shows `n/3` plus three icons: identity, legal residence, address — matching the checklist on the case detail page.

---

## Demo walkthrough

1. Open a case with at least one attached PDF or image (from Phase 1–2 upload).
2. On the right-hand document panel, click the file → inline preview appears below the list.
3. Click **fullscreen** → dialog opens with the same preview.
4. Click **Download** → browser opens or saves the file.
5. Return to the case list → **Progress** column shows `2/3` (or similar) with per-step icons.

---

## Tests

```bash
dotnet test --configuration Release --filter "Category!=PostgreSQL"
```

Expected: **77 tests passing** (unchanged — no new automated tests in this phase; download is covered manually).

---

## Carries forward

- Phase 5+ case detail pages should keep the document panel pattern; new intake sections go in the left column.
- A future `AppDocumentPreview` wrapper (design-system Wave 2) can subsume `RegistrationCaseDocumentPanel` internals without changing the layout contract.
- Print support and the full two-pane viewer template remain planned for Phase 8 — see [06-page-templates.md](../design-system/06-page-templates.md#8-document-viewer-phase-8).

---

## Related documents

- [ROADMAP.md](../ROADMAP.md)
- [phase-4-address-household.md](./phase-4-address-household.md)
- [download-document.md](../features/registration/download-document.md)
- [attach-document.md](../features/registration/attach-document.md)
- [list-registration-cases.md](../features/registration/list-registration-cases.md)
