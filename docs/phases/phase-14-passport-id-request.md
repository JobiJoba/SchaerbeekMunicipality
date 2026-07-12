# Phase 14 — Passport / ID card request

- **Status:** Complete
- **Goal:** Simplified **citizen document request** workflow — apply for a Belgian passport or national ID card after the person is registered.
- **Maps to IDEA:** Reception visit reason “Passport renewal”; municipal role in identity document applications (demo-grade).

---

## Summary

Registered residents request a passport or eID card. Officers verify register data, collect **biometric appointment** metadata (stub), check **fee payment** (stub), track **production status**, and mark the document **issued** with a simulated document number.

This is a **service case**, not a population register entry procedure.

---

## Bounded context

| Context | Role |
|---------|------|
| **IdentityDocuments** (new) | `DocumentRequestCase` aggregate |
| **Registration / Person** (existing) | Read-only source of truth for identity fields |

---

## Domain

### Aggregate: `DocumentRequestCase`

| Field / concept | Description |
|-----------------|-------------|
| `DocumentRequestCaseId` | Strongly typed identifier |
| `PersonId` | Applicant |
| `RequestType` | `Passport`, `IdentityCard`, `PassportRenewal`, `IdentityCardRenewal` |
| `Status` | `Submitted` → `InProduction` → `ReadyForCollection` → `Issued` / `Cancelled` |
| `PhotoAttached` | Boolean + document reference |
| `FeePaid` | Stub boolean + reference |
| `IssuedDocumentNumber` | Set on completion |
| `RequestedAt` / `IssuedAt` | Timestamps |

### Rules

- Person must be in population register with valid NR number.
- Minor requests require linked parent (read from household).
- Cannot issue without photo attachment (demo rule).

---

## Slices

| Slice | Notes |
|-------|-------|
| `OpenDocumentRequestCase` | Select person + request type |
| `ListDocumentRequestCases` / `GetDocumentRequestCase` | Queue for officers |
| `AttachApplicantPhoto` | Reuse document storage |
| `RecordFeePayment` | Stub payment reference |
| `AdvanceDocumentRequestStatus` | Officer moves through workflow |
| `IssueDocument` | Terminal; sets document number |
| `CancelDocumentRequest` | With reason |

---

## UI

- `/identity-documents/requests` — queue with status chips (`Submitted`, `In production`, …).
- Case detail: applicant summary (from person read), photo upload, fee checkbox, status actions.
- Case locking: same `CaseLockActions` / `CaseLockBar` pattern as Phase 8.1 — read-only when locked to a colleague; **Take case** only when the case is available to claim.
- Optional: citizen-facing “track my request” read-only page (low priority).

---

## Demo

Registered person requests eID renewal → upload photo → mark fee paid → advance to issued → stub document number `BE-2026-XXXX`.

---

## Tests

- Cannot open request for unregistered person.
- Issue blocked without photo.
- Status transitions enforce forward-only path (except cancel).

---

## Out of scope

- Real FPS Foreign Affairs / eID integration.
- Biometric capture hardware.
- FR / NL localization.

---

## Related documents

- [phase-8 certificates](./phase-8.1-role-boundaries-case-locking.md) — printable outputs pattern (Phase 8 certificates are register extracts, not travel documents)
