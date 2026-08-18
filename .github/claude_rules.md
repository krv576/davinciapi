ROLE

You are my Da Vinci CRD 2.2.1 Inferno Conformance Engineer.

OBJECTIVE

Fix ONE Inferno failure at a time while preserving existing business logic.

Never attempt to fix multiple Inferno tests in one run.

============================================================

PROJECT

This is a .NET 8 ASP.NET Core implementation of a Da Vinci CRD Server.

The following components are considered production-ready:

- CoverageRequirementDiscoveryService
- Coverage Rule Engine
- Repository Layer
- EF Core Persistence
- PAS
- DTR
- Provider API
- FHIR Serialization

Assume these are correct.

Do not modify them unless absolutely required by the specification.

============================================================

TOKEN OPTIMIZATION RULES

DO NOT scan the entire repository.

DO NOT analyze unrelated files.

DO NOT search recursively unless required.

Only inspect files directly related to the current Inferno failure.

Maximum files to inspect initially: 5.

Only inspect additional files if the current files prove insufficient.

Never reread files already analyzed unless I ask.

Do not summarize the repository.

Do not explain Da Vinci CRD unless necessary.

Keep responses concise.

============================================================

SOURCE OF TRUTH

Use only:

1. Current Inferno failure
2. Da Vinci CRD 2.2.1
3. CDS Hooks Specification

Never predict future failures.

============================================================

WORKFLOW

When I paste an Inferno failure:

STEP 1

Identify

- Test ID
- Test Name
- Exact failure

STEP 2

Determine which component owns the failure.

Examples

Discovery DTO

Program.cs

Card Builder

FHIR Builder

JSON serialization

Middleware

Endpoint

Validator

STEP 3

Inspect ONLY those files.

Do not inspect anything else.

STEP 4

Compare implementation against the specification.

Determine whether the issue is

- Missing field
- Wrong JSON
- Missing extension
- Serialization
- HTTP behavior
- FHIR conformance

STEP 5

Produce ONLY

Problem

Root Cause

Files to modify

Minimal Fix

Specification reference

STEP 6

Implement ONLY that fix.

Do not refactor.

Do not improve unrelated code.

============================================================

OUTPUT FORMAT

Return only

Problem

Root Cause

Files Modified

Exact Code Changes

Why Inferno will now pass

Then STOP.

Do not continue.

Wait for the next Inferno failure.

============================================================

IMPORTANT

Never implement fixes for tests that have not yet failed.

Never speculate.

Never redesign working business logic.

Always optimize for the smallest possible code change.



the following is the v2.2.1 CRD test suite specifications

Visit the HL7 website Visit the FHIR website Search FHIR Visit the Da Vinci website
Da Vinci - Coverage Requirements Discovery
2.2.1 - STU 2.2  United States of America flag

IG Home
Background
Specification
FHIR Artifacts
Base Specs
Support
Change Log
Table of ContentsCRD IG Home Page < prev | bottom | next >
This page is part of the Da Vinci Coverage Requirements Discovery (CRD) FHIR IG (v2.2.1: STU 2.2) based on FHIR (HL7® FHIR® Standard) R4. This is the current published version in its permanent home (it will always be available at this URL). For a full list of available versions, see the Directory of published versions

CRD IG Home Page
Official URL: http://hl7.org/fhir/us/davinci-crd/ImplementationGuide/hl7.fhir.us.davinci-crd	Version: 2.2.1	Highlight text changes?  
IG Standards status: Trial-use Active as of 2026-03-27	Maturity Level: 4	Computable Name: CoverageRequirementsDiscovery
Other Identifiers: OID:2.16.840.1.113883.4.642.40.18

Overview
Systems
Content and Organization
Dependencies
Intellectual Property Considerations
This STU update of the specification reflects changes based on implementer feedback about the Coverage Requirements Discovery (hereafter, CRD) specification arising from detailed review, connectathons and implementation experience. "STU notes" call out additional key considerations where feedback is desired.

This specification is a Standard for Trial Use. It is expected to continue to evolve and improve through connectathon testing and feedback from early adopters.

Feedback is welcome. Requests for change may be submitted through the FHIR change tracker indicating "US Da Vinci CRD" as the specification. Questions should be raised on the CRD Zulip stream.

This implementation guide is dependent on other specifications. Please submit any comments you have on these base specifications as follows:

Feedback on CDS Hooks should be posted to the FHIR change tracker with "CDS Hooks" as the specification.
Feedback on the FHIR Core specification should be submitted to the FHIR change tracker with "FHIR Core" as the specification.
Feedback on the US Core profiles should be submitted to the FHIR change tracker with "US Core" as the specification.
Individuals interested in participating in the Da Vinci Burden Reduction project or other HL7 Da Vinci projects can find information about Da Vinci here.

A summary of the major changes from the previous release can be found here.

One of the changes in this release is migrating a number of codes from a temporary 'custom' code system in this IG to a standard code system that will eventually allow codes to evolve without a new release of this IG and to be shared with other specifications. Support for the CRD-specific code system alongside the new code system continues to be mandated in this release to ease transition. The set of codes for one value set - Coverage Assertion Reasons did not complete it's migration in time for this release. See the comment on that value set for recommendations.
Overview

The process of billing a patient's insurance provider is complex and costly, particularly in the United States. Healthcare providers work with a range of payers who provide coverage for the products and clinical services provided to patients. Each payer offers distinct insurance plans for healthcare products and services, and each has its own unique process to determine whether each service is necessary and appropriate. These processes have many different requirements for documentation, prior authorization, or other approval steps. Claims submitted for payment that do not meet payer requirements will typically be denied, which may result in service delay, resubmission, or appeal. These delays and additional processes may result in negative health outcomes or financial costs for patients, as well as financial and productivity losses for providers.
This Coverage Requirements Discovery (CRD) implementation guide defines a workflow in which a payer makes coverage requirement information available to a healthcare provider within the provider's software system at the point of care where treatment decisions are made. This will help clinicians and administrative staff make informed recommendations to their patients and meet payer submission requirements.

This implementation guide supports both Protected Health Information (PHI)-specific and non-PHI mechanisms for CRD to meet the needs and privileges of different payer organizations. These mechanisms will allow payers to share a wide variety of information with providers in a context-sensitive manner including:

updates to coverage information
alternative (e.g. first-line, lower-cost, etc.) services or products
documentation requirements and rules related to coverage
forms and templates to complete
indications of whether a therapy is covered and if prior authorization is required, including propagating this information into the relevant order/appointment
This implementation guide is designed to allow for initial support of basic capabilities and to subsequently build new features over time.


The scope of this specification has increased to also support prior authorization process earlier in the workflow by allowing prior authorization to be returned during the CRD interaction. Specifically:

On Feb 28, 2024, the Office of Burden Reduction and Health Informatics (OBRHI) National Standards Group (NSG) announced an enforcement discretion that they would not enforce the requirement to use the X12 278 for prior authorization if the covered entities were using the FHIR-based Prior Authorization API as described in the CMS Interoperability and Prior Authorization final rule (CMS-0057-F). This allows payers to return a prior authorization number for use in the X12 837 in coverage extension of the CRD and DTR IGs or as part of the all-FHIR exchange of the Prior Authorization Response Bundle in the PAS IG. For CRD, this specifically means that the satisfied-pa-id in the Coverage Information extension can be used as an X12 prior authorization number.
Systems
This implementation guide sets expectations for two types of systems:

CRD clients are typically systems that healthcare providers use at the point of care, including electronic medical records systems, pharmacy systems, and other provider and administrative systems used for ordering, documenting, and executing patient-related services. Users of these systems need coverage requirements information to support care planning.

Examples of potential CRD clients include EHRs, EMRs, practice management systems, scheduling systems, patient registration systems, etc.

The CRD client may actually involve multiple systems. For example, the systems that handle order entry may be different from what is used for appointment booking and different again from the system that exposes information over the FHIR interface. It is possible that a provider environment might use an intermediary to coordinate CRD client calls from multiple systems. Such an architecture is sufficient provided that:

Calls are triggered from within the system the user is interacting with at the time when the 'hook event' (entering an order, booking an appointment, etc.) occurs.
Cards returned are displayed to the user, or in the event of system actions, user-notifications associated with the system actions are presented to the user within the same application.
The 'access token' and FHIR endpoint exposed to the CRD server has access to all relevant data, independent of which physical data store it resides.
The intermediary could take responsibility for the FHIR interface, such as determining which payer should receive a coverage request.
There are three distinct sets of capabilities for CRD clients, one for USCDI v1 (US-Core 3.1.1), one for USCDI v3 (US-Core 6.1.0), and one for USCDI v4 (US-Core 7.0.0). Typically, a client would support only one of these, based on which US Core release the client supports internally. There is a single CRD server set of capabilities which must be able to handle data from any of the three supported USCDI versions.

When CRD clients are made up of multiple systems, there will be orchestration requirements to allow each system to interact in a way that together they appear as a single monolithic system from the perspective of the CRD server. This IG provides some discussion of this on the electronic prior authorization (ePA) Coordinators page, though it does not yet provide any standardization about how components should interoperate to achieve the intended monolithic behavior. If there is industry interest, future releases of this IG may work to standardize some of these "intra-client" interactions.
CRD servers (or servers) are systems that act on behalf of payer organizations to share information with healthcare providers about rules and requirements related to healthcare products and services covered by a patient's health plan. A CRD server will provide coverage information related to one or more insurance plans. CRD servers are a type of CDS service as defined in the CDS Hooks Specification.

There are is a single set of capabilities for CRD servers that spans USCDI v1 (US-Core 3.1.1) USCDI v3 (US-Core 6.1.0), and USCDI v4 (US-Core 7.0.0) expectations. Payers will need to handle content from any of the releases, as CRD clients will be transitioning support for the versions at different times - and in some cases may provide content that spans a mixture of versions.

Content and Organization
This implementation guide (and the menu for it) is organized into the following sections:

Background - Supporting informative pages that do not set conformance expectations
Reading this IG points to key pages in the FHIR spec and other source specifications that must be understood to understand this guide
Use Cases describes the intent of the implementation guide, gives examples of its use, and provides a high-level overview of expected process flow
Project and Participants gives a high-level overview of Da Vinci and identifies the individuals and organizations involved in developing this implementation guide
Burden Reduction identifies related specifications this implementation guide builds upon that developers should read and understand prior to implementing this specification
ePA Coordinators acknowledges that neither the payer nor provider systems involved in CRD are monolithic and shows how the various components of provider and payer systems might interact with "ePA Coordinator" systems to satisfy the requirements of this IG
Operational Recommendations highlights topics that organizations should take into account when implementing the specification that fall outside the boundaries of conformance validation.
Specification - Pages that set conformance expectations
Conformance Expectations defines base language and expectations for declaring conformance with the guide
Privacy, Safety, and Security covers considerations around data access, protection, and similar concepts that apply to all implementations
Foundational Guidance covers high-level conformance expectations that apply to all implementations
Deviations and Enhancements covers detailed implementation requirements and conformance expectations that are independent of specific hooks or cards
Supported Hooks identifies the expectations for support for specific CDS hooks
Hook Response Profiles defines patterns for CDS Hooks cards and system actions that can be returned as part of this specification
Implementation Guidance provides recommendations for implementation that fall outside the technical scope of the specification
CRD Metrics provides a logical model describing how to capture data that may be relevant to measuring or reporting on CRD use
FHIR Artifacts
Artifacts Overview introduces and provides links to the profiles, search parameters and other FHIR artifacts used in this implementation guide
Additional links point to complete lists of all artifacts defined in this guide as well as ancestor guides
Base Specifications - Quick links to the various specifications this guide derives from
Support - Links to help with use of this guide
Discussion Forum is a place to ask questions about the guide, discuss potential issues, and search through prior discussions
Project Home includes information about project calls, agendas, past minutes, and instructions for how to participate
Implementer Support provides information about reference implementations, resources for testing, known errata, regulatory considerations, and practical implementation pathways
Project Dashboard shows new and historical issues that have been logged against the specification, proposed dispositions, unapplied changes, etc.
Propose a Change allows formal submission of requests for change to the specification. (Consider raising the issue on the discussion forum first.)
Downloads allows downloading this and other specifications, as well as other useful files
Dependencies
This guide is based on the FHIR R4 specification that is mandated for use in the U.S. It also leverages the SMART on FHIR specification for CRD clients that opt to use that approach for "what-if" scenarios.

In addition, this guide also relies on several ancestor implementation guides:

Implementation Guide	Version(s)	Reason
CDS Hooks	3.0.0-ballot	
The CDS Hooks specification the CRD architecture is based on
CDS Hooks Library	1.0.1	
Provides the hook definitions for CDS Hooks
Da Vinci Health Record Exchange (HRex)	1.2.0	
Defines common conformance rules across all Da Vinci IGs, as well as additional constraints and profiles beyond U.S. Core
Extensions for Using Data Elements from FHIR R5 in FHIR R4	0.1.0	
Needed to pre-adopt R5 Questionnaire elements in R4
FHIR Extensions Pack	5.3.0-ballot-tc1	Imported by Structured Data Capture (and potentially others)
5.2.0	
Automatically added as a dependency - all IGs depend on the HL7 Extension Pack
FHIR R4 package : Core	4.0.1	Imported by HL7 Terminology (THO) (and potentially others)
FHIR Tooling Extensions IG	1.1.2	
Defines the CDS Hooks logical models
HL7 Terminology (THO)	7.1.0	
Automatically added as a dependency - all IGs depend on HL7 Terminology
7.0.1	Imported by CDS Hooks (and potentially others)
6.5.0	Imported by FHIR Extensions Pack (and potentially others)
6.2.0	Imported by CDS Hooks Library (and potentially others)
5.5.0	Imported by US Core (and potentially others)
Public Health Information Network Vocabulary Access and Distribution System (PHIN VADS)	0.12.0	Imported by US Core (and potentially others)
SMART App Launch	2.0.0	Imported by US Core (and potentially others)
Structured Data Capture	4.0.0	
Defines expectations for Questionnaires prompted by cards
3.0.0	Imported by US Core (and potentially others)
US Core	7.0.0	
Defines USCDI v4 EHR expectations on a range of resources that will be passed to and/or queried by CRD servers.
6.1.0	
Defines USCDI v3 EHR expectations on a range of resources that will be passed to and/or queried by CRD servers
3.1.1	
Defines USCDI v1 EHR expectations on a range of resources that will be passed to and/or queried by CRD servers.
Value Set Authority Center (VSAC)	0.19.0	
Uses the latest version of the VSAC codes
0.18.0	Imported by US Core (and potentially others)
This implementation guide defines additional constraints and usage expectations above and beyond the information found in these base specifications.

Intellectual Property Considerations
This implementation guide and the underlying FHIR specification are licensed as public domain under the FHIR license. The license page also describes rules for the use of the FHIR name and logo.

CPT © Copyright 2026 American Medical Association. All rights reserved. AMA and CPT are registered trademarks of the American Medical Association.
This publication includes IP covered under the following statements.

© Copyright 2022 American Medical Association
Show Usage
ISO maintains the copyright on the country codes, and controls its use carefully. For further details see the ISO 3166 web page: https://www.iso.org/iso-3166-country-codes.html
Show Usage
Licensing information can be found here These codes are listed within the UB-04 Data Specifications Manual. The Official UB-04 Data Specifications Manual, copyrighted by the American Hospital Association, is the only official source of UB-04 billing information adopted by the National Uniform Billing Committee. No other publication—governmental or private/commercial—can be considered authoritative. The AHA wants to make you aware that the use of codes, descriptions, or any other content contained in the manual to be used in a software application, publication, or any other derivative work must be properly licensed by the AHA. If your organization uses or intends to use any of the codes or other related content from the manual in this manner, please contact the AHA’s licensing manager, Tim Carlson, at 312.893.6816 or tcarlson@aha.org
Show Usage
The UCUM codes, UCUM table (regardless of format), and UCUM Specification are copyright 1999-2009, Regenstrief Institute, Inc. and the Unified Codes for Units of Measures (UCUM) Organization. All rights reserved. https://ucum.org/trac/wiki/TermsOfUse
Show Usage
This material contains content from LOINC. LOINC is copyright © 1995-2020, Regenstrief Institute, Inc. and the Logical Observation Identifiers Names and Codes (LOINC) Committee and is available at no cost under the license. LOINC® is a registered United States trademark of Regenstrief Institute, Inc.
Show Usage
This material contains content that is copyright of SNOMED International. Implementers of these specifications must have the appropriate SNOMED CT Affiliate license - for more information contact https://www.snomed.org/get-snomed or info@snomed.org.
Show Usage
This material derives from the HL7 Terminology (THO). THO is copyright ©1989+ Health Level Seven International and is made available under the CC0 designation. For more licensing information see: https://terminology.hl7.org/license.html
Show Usage
Used by permission of HL7 International, all rights reserved Creative Commons License
Show Usage
Using RxNorm codes of type SAB=RXNORM as this specification describes does not require a UMLS license. Access to the full set of RxNorm definitions, and/or additional use of other RxNorm structures and information requires a UMLS license. The use of RxNorm in this specification is pursuant to HL7's status as a licensee of the NLM UMLS. HL7's
