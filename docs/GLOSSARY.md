# Glossary

Belgian municipal and immigration terms used in this project. English is primary in code; UI may show FR/NL labels later.

## Registers

| Term | Code / enum | Description |
|------|-------------|-------------|
| **Population Register** | `RegisterTarget.PopulationRegister` | Primary register for Belgian nationals and persons with established residence rights |
| **Foreigners Register** | `RegisterTarget.ForeignersRegister` | Non-Belgians registered with residence documentation |
| **Waiting Register** | `RegisterTarget.WaitingRegister` | Persons awaiting decision or verification |
| **Special Register** | `RegisterTarget.SpecialRegister` | Diplomats and others under special rules |
| **National Register** | — | Central population register (RN/RR); municipalities read/write via official systems — **stubbed** in this project |
| **BIS number** | `BisNumber` | Identification number for persons not (yet) in the National Register; may convert to NR number |

## Identification numbers

| Term | Description |
|------|-------------|
| **National Register number** (NN/NISS) | 11-digit Belgian identification number assigned upon registration |
| **BIS number** | Temporary or administrative identifier before full NR registration |

## Roles (actors)

| Term | Code role | Description |
|------|-----------|-------------|
| **Reception officer** | `ReceptionOfficer` | Front desk; determines visit reason and routing |
| **Population officer** | `PopulationOfficer` | Handles registration, identity, address, decision |
| **Back office officer** | `BackOfficeOfficer` | Read-only operational reports and KPIs (no case editing) |
| **Police clerk** | `PoliceClerk` | Records residence verification outcomes |
| **Citizen** | — | Person requesting registration (not a system user in early phases) |

## Residence documents (simplified)

> **Accuracy disclaimer:** descriptions below are simplified for this educational project. Belgian residence card letters were reorganised in 2021 (C→K, D→L, E→EU, E+→EU+). Verify against official Immigration Office (IBZ/DVZ) sources before encoding legal rules as code.

| Term | Description |
|------|-------------|
| **A Card** | Limited-duration residence permit (non-EU) |
| **B Card** | Unlimited-duration residence permit (non-EU) |
| **EU / EU+ Card** | Registration certificate / permanent residence for EU citizens (formerly E / E+) |
| **F / F+ Card** | Family member of an EU or Belgian citizen (F+ = permanent) |
| **H Card** | European Blue Card (highly skilled worker) |
| **K Card** | Settled status (formerly C card) |
| **L Card** | EU long-term resident (formerly D card) |
| **Annex 15, 19, 19ter, 20, 25, 26, 35** | Immigration Office annex documents with distinct legal meanings |
| **Visa D** | Long-stay visa |
| **Visa C** | Short-stay visa |
| **Electronic residence card** | Chip card with PIN/PUK |

## Residence categories

| Term | Enum | Description |
|------|------|-------------|
| **EU citizen** | `EuCitizen` | Freedom of movement; lighter documentation |
| **Non-EU worker** | `NonEuWorker` | Employment-based stay |
| **Student** | `Student` | Study purpose |
| **Researcher** | `Researcher` | Research appointment |
| **Family reunification** | `FamilyReunification` | Joining family member legally in Belgium |
| **Highly skilled worker** | `HighlySkilledWorker` | EU Blue Card / skilled migration |
| **Diplomat** | `Diplomat` | Special Register; passport or diplomatic card; police verification waived (Phase 18) |
| **Refugee** | `Refugee` | Recognised refugee status |
| **Subsidiary protection** | `SubsidiaryProtection` | Alternative protection status |
| **Temporary protection** | `TemporaryProtection` | e.g. mass displacement schemes |
| **Cross-border worker** | `CrossBorderWorker` | Lives abroad, works in Belgium |

## Address & housing

| Term | Description |
|------|-------------|
| **Domicile / official address** | Address recorded in the register; legal residence for admin purposes |
| **Reference address** | Municipality-hosted address when person has no fixed domicile (`AddressDeclarationType.ReferenceAddress`, Phase 18); police verification still applies |
| **Housing situation** | Owner, tenant, living with parents, student housing, shelter, hotel, … |
| **Household composition** | List of persons living at the same address with relationships |

## Civil status

| Term | Description |
|------|-------------|
| **Civil status** | Single, married, separated, divorced, widowed, registered partnership |
| **Marriage recognition** | Process to recognise foreign marriage under Belgian law |
| **Legalisation** | Authentication of foreign public document |
| **Apostille** | Simplified legalisation (Hague Convention countries) |

## Workflow terms

| Term | Description |
|------|-------------|
| **Registration case** | Administrative file for one registration procedure (`RegistrationCase`) |
| **First registration** | Initial registration in Belgium |
| **Provisional registration** | Registration pending police verification |
| **Police residence verification** | Police confirm person lives at declared address |
| **Immigration Office** | Federal authority for residence decisions — **stubbed** |
| **Residence certificate** | Proof of registration / domicile issued to citizen |
| **Household composition certificate** | Document listing household members |

## Police verification outcomes

| Term | Enum | Description |
|------|------|-------------|
| **Confirmed** | `Confirmed` | Person found at address |
| **Not found** | `NotFound` | Person not present |
| **Address incorrect** | `AddressIncorrect` | Wrong address provided |
| **Mailbox only** | `MailboxOnly` | Only mailbox, not actually residing |
| **Empty dwelling** | `EmptyDwelling` | No one living at address |
| **Refused access** | `RefusedAccess` | Police could not enter / verify |
| **Incomplete** | `Incomplete` | Needs second visit |

## Case statuses (project-specific)

| Term | Description |
|------|-------------|
| **Person file** | Read-only consolidated citizen profile (identity, domicile, household, cases, certificates, history) composed from multiple workflows — see Phase 16 |

Data completeness (identity, residence, address, documents) is tracked by a **checklist** on the case, not by statuses — see ARCHITECTURE.md.

| Status | Meaning |
|--------|---------|
| `Intake` | Case open; officer collects identity, residence, address, household, and documents in any order |
| `AwaitingPoliceVerification` | Sent to police for residence check |
| `UnderReview` | Police result recorded; officer evaluates the four core decisions |
| `Approved` | Decision positive, pending formal registration |
| `Registered` | Successfully entered in register (terminal) |
| `Rejected` | Registration refused (terminal) |
| `Suspended` | Procedure paused (missing documents, investigation, …); resumable |

## Administrations (notification stubs)

| Body | Purpose |
|------|---------|
| **SPF Finances / Tax** | Tax registration |
| **Social security (RSZ/ONSS)** | Employment and benefits |
| **Health insurance fund** | Mutuelle / ziekenfonds affiliation |
| **Regional authorities** | Region-specific obligations |

---

## Related documents

- [DOMAIN.md](./DOMAIN.md) — how terms map to aggregates
- [IDEA.md](../IDEA.md) — full process narrative
