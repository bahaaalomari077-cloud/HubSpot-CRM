--
-- PostgreSQL database dump
--

\restrict LU8MialxrS66zJaTUEIaYaaALDOxEAX0VpSQQkjx0MgVEYfrCOp5hYXP6YmPJRs

-- Dumped from database version 17.9
-- Dumped by pg_dump version 17.9

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: activities; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.activities (
    "Id" uuid NOT NULL,
    "Subject" character varying(256) NOT NULL,
    "Description" text,
    "Type" integer NOT NULL,
    "Status" integer NOT NULL,
    "StartTime" timestamp with time zone,
    "Deadline" timestamp with time zone,
    "IsCompleted" boolean NOT NULL,
    "ContactId" uuid,
    "ApplicationId" uuid,
    "AssignedToId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedById" uuid,
    "UpdatedAt" timestamp with time zone,
    "UpdatedById" uuid,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone
);


ALTER TABLE public.activities OWNER TO postgres;

--
-- Name: applications; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.applications (
    "Id" uuid NOT NULL,
    "Title" character varying(256) NOT NULL,
    "Stage" integer NOT NULL,
    "StageSemantic" integer NOT NULL,
    "IsClosed" boolean NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "CurrencyCode" character(3) NOT NULL,
    "Probability" integer NOT NULL,
    "BeginDate" timestamp with time zone,
    "CloseDate" timestamp with time zone,
    "ContactId" uuid,
    "CompanyId" uuid,
    "AssignedToId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedById" uuid,
    "UpdatedAt" timestamp with time zone,
    "UpdatedById" uuid,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "MovedAt" timestamp with time zone,
    "PreviousStageId" integer
);


ALTER TABLE public.applications OWNER TO postgres;

--
-- Name: companies; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.companies (
    "Id" uuid NOT NULL,
    "Name" character varying(256) NOT NULL,
    "Industry" character varying(120),
    "Website" character varying(256),
    "Phone" character varying(40),
    "Email" character varying(256),
    "AddressCity" character varying(120),
    "AddressCountry" character varying(120),
    "AnnualRevenue" numeric(18,2),
    "EmployeeCount" integer,
    "AssignedToId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedById" uuid,
    "UpdatedAt" timestamp with time zone,
    "UpdatedById" uuid,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone
);


ALTER TABLE public.companies OWNER TO postgres;

--
-- Name: contacts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.contacts (
    "Id" uuid NOT NULL,
    "FirstName" character varying(120) NOT NULL,
    "LastName" character varying(120) NOT NULL,
    "Email" character varying(256),
    "Phone" character varying(40),
    "Position" character varying(160),
    "CompanyId" uuid,
    "AssignedToId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedById" uuid,
    "UpdatedAt" timestamp with time zone,
    "UpdatedById" uuid,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "Status" integer DEFAULT 0 NOT NULL,
    "NextAction" character varying(300),
    "NextActionDate" timestamp with time zone
);


ALTER TABLE public.contacts OWNER TO postgres;

--
-- Name: messages; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.messages (
    "Id" uuid NOT NULL,
    "Channel" integer NOT NULL,
    "Direction" integer NOT NULL,
    "Subject" character varying(256),
    "Body" text NOT NULL,
    "SentAt" timestamp with time zone NOT NULL,
    "IsRead" boolean NOT NULL,
    "ContactId" uuid NOT NULL,
    "UserId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedById" uuid,
    "UpdatedAt" timestamp with time zone,
    "UpdatedById" uuid,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "AttemptCount" integer DEFAULT 0 NOT NULL,
    "FailureReason" character varying(1000),
    "LastAttemptAt" timestamp with time zone,
    "ProviderMessageId" character varying(256),
    "Recipient" character varying(320),
    "Status" integer DEFAULT 0 NOT NULL
);


ALTER TABLE public.messages OWNER TO postgres;

--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    "Id" uuid NOT NULL,
    "FirstName" character varying(120) NOT NULL,
    "LastName" character varying(120) NOT NULL,
    "Email" character varying(256) NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CreatedById" uuid,
    "UpdatedAt" timestamp with time zone,
    "UpdatedById" uuid,
    "IsDeleted" boolean NOT NULL,
    "DeletedAt" timestamp with time zone
);


ALTER TABLE public.users OWNER TO postgres;

--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260921102018_InitialCreate	10.0.12
20260921111645_AddMessageStatusTracking	10.0.12
20260921123801_AddStageTransitionTracking	10.0.12
20260921124339_AddContactStatus	10.0.12
20260921125425_AddActivityContactCreatedAtIndex	10.0.12
20260921130836_AddContactNextAction	10.0.12
\.


--
-- Data for Name: activities; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.activities ("Id", "Subject", "Description", "Type", "Status", "StartTime", "Deadline", "IsCompleted", "ContactId", "ApplicationId", "AssignedToId", "CreatedAt", "CreatedById", "UpdatedAt", "UpdatedById", "IsDeleted", "DeletedAt") FROM stdin;
930b2e80-c550-4dc1-b82d-dfdb39e9d4b1	Note	Left a voicemail	7	1	\N	\N	f	37dd8a62-1994-494a-8404-f6a83e720d13	\N	\N	2026-09-21 14:34:25.151052+03	\N	2026-09-21 14:34:44.612291+03	\N	t	2026-09-21 14:34:44.612094+03
14f19966-f058-4bad-ab9b-914ed8fdf1c3	Intro call	Discussed pricing	2	1	\N	\N	f	37dd8a62-1994-494a-8404-f6a83e720d13	\N	\N	2026-09-21 14:34:25.317019+03	\N	2026-09-21 14:34:55.635488+03	\N	t	2026-09-21 14:34:55.635343+03
394d7708-d8ad-4ed0-bd4c-3fd63549eb91	Intro call	Discussed scope	2	1	\N	\N	f	8a9e5ea5-5765-4c66-b238-008297f2a26d	\N	\N	2026-09-21 15:38:46.273993+03	\N	2026-09-21 15:40:59.438004+03	\N	t	2026-09-21 15:40:59.43778+03
0d5f7b29-080a-4bef-9664-9d1186082178	Office meeting	Met at their HQ	1	1	\N	\N	f	3b21d012-9087-4ae9-bd98-45962e49fafa	\N	\N	2026-09-21 15:44:26.413672+03	\N	2026-09-21 15:45:04.121777+03	\N	t	2026-09-21 15:45:04.121612+03
776dcd6f-56cf-4000-b62c-b3efdec86e12	Intro call	Explained the offer	2	1	\N	\N	f	3b21d012-9087-4ae9-bd98-45962e49fafa	\N	\N	2026-09-21 15:44:26.3766+03	\N	2026-09-21 15:45:04.159287+03	\N	t	2026-09-21 15:45:04.15922+03
18b16463-5305-41b7-add5-5112e0b42f18	Note	Referred by a partner	7	1	\N	\N	f	3b21d012-9087-4ae9-bd98-45962e49fafa	\N	\N	2026-09-21 15:44:26.326568+03	\N	2026-09-21 15:45:04.195377+03	\N	t	2026-09-21 15:45:04.195317+03
e8052272-7cda-489a-b6f2-7cff83dfd6fb	Note	Referred by a partner. Wants a demo next week.	7	1	\N	\N	f	baca2cd0-274d-41d1-910d-96a97f13e07f	\N	\N	2026-09-21 15:49:58.529033+03	\N	2026-09-21 15:50:55.031403+03	\N	t	2026-09-21 15:50:55.031335+03
686eab45-f4fe-4d24-b2ee-bd219ff880f3	Note	log test	7	1	\N	\N	f	8572c1b8-df47-4dda-b98e-fc92be117486	\N	\N	2026-09-21 15:58:21.266933+03	\N	2026-09-21 16:00:54.104287+03	\N	t	2026-09-21 16:00:54.104117+03
cc36a0ad-fb0b-4897-8678-6ea19c34fc44	Call	d	2	1	\N	\N	f	7a7b30bf-5dca-4c9d-b822-c0c578fca6b3	\N	\N	2026-09-21 15:59:33.013523+03	\N	2026-09-21 16:00:54.255923+03	\N	t	2026-09-21 16:00:54.255834+03
76b28a07-018b-45f3-a347-edc9660036d1	Call	d	2	1	\N	\N	f	4167818c-c1e5-4859-8177-ed0254b5795c	\N	\N	2026-09-21 16:00:37.737735+03	\N	2026-09-21 16:00:54.395987+03	\N	t	2026-09-21 16:00:54.395904+03
259a0331-5aa6-41da-8bad-30e4eb21a722	Note	Called back — they want pricing for 50 seats.	7	1	\N	\N	f	0a78018c-3d9f-468e-bbb2-7bdf505edfd3	\N	\N	2026-09-21 16:13:07.395835+03	\N	2026-09-21 16:13:36.057073+03	\N	t	2026-09-21 16:13:36.056927+03
a3a4c0a1-318b-4874-8de5-3676fe00dabf	Intro call	Discussed pricing	2	1	\N	\N	f	0a78018c-3d9f-468e-bbb2-7bdf505edfd3	\N	\N	2026-09-21 16:12:08.061254+03	\N	2026-09-21 16:13:36.098439+03	\N	t	2026-09-21 16:13:36.098374+03
09cf8679-f24b-4e29-b9d0-c65ff2cab429	Note	Referred by partner	7	1	\N	\N	f	c9209567-d522-4ac7-b45f-558e92e4a4c1	\N	\N	2026-09-21 16:12:08.109938+03	\N	2026-09-21 16:13:36.242435+03	\N	t	2026-09-21 16:13:36.242375+03
14000fd8-ef0c-4231-86be-1cfd16d7cf61	Office meeting	\N	1	1	\N	\N	f	93e0015b-532a-45a8-b103-01a23d7d264e	\N	\N	2026-09-21 16:12:08.145384+03	\N	2026-09-21 16:13:36.391292+03	\N	t	2026-09-21 16:13:36.391241+03
4963990b-2f21-464b-8db2-8a3705eea98b	Edited call	revised version	2	2	\N	\N	t	4a5ed0a4-1c37-42ab-8c90-cc3152438e97	\N	\N	2026-09-21 16:50:46.16001+03	\N	2026-09-21 16:50:46.407554+03	\N	t	2026-09-21 16:50:46.406935+03
085169c2-e6d1-4186-835f-27f57909c90a	Note	Called, asked for a callback Monday	7	1	\N	\N	f	578f8b67-d9f6-4536-a765-d5b9ea4bbf84	\N	\N	2026-09-21 17:05:51.254308+03	\N	2026-09-21 17:07:14.670621+03	\N	t	2026-09-21 17:07:14.670539+03
d7178183-c269-4ad8-8a7e-36123a242d4a	Note	Confirmed the meeting for tomorrow	7	1	\N	\N	f	4bbe083e-b3aa-48f0-8c69-e8c5ff4ceef5	\N	\N	2026-09-21 17:09:16.474083+03	\N	2026-09-21 17:09:46.183334+03	\N	t	2026-09-21 17:09:46.183281+03
0c6b1a72-7d5c-4bd5-ab9e-99eeb16e2db2	Edited	\N	2	2	\N	\N	t	f8f9a79e-f128-48a9-ae62-6ed32d227069	\N	\N	2026-09-21 21:50:25.125088+03	\N	2026-09-21 21:51:30.714013+03	\N	t	2026-09-21 21:51:30.713646+03
1c07232c-726d-47f9-a75a-95b93dcb10fd	Intro call	Discussed scope	2	1	\N	\N	f	e8d1b76d-83ec-416e-8fb8-e9caf1e81231	\N	\N	2026-09-21 21:51:43.689056+03	\N	2026-09-21 21:55:25.112588+03	\N	t	2026-09-21 21:55:25.112524+03
de37aca1-0af3-4f35-9cc5-17b483c59a90	Note	to be deleted	7	1	\N	\N	f	b63ee261-855b-4957-9504-403e85275987	\N	\N	2026-09-21 22:00:59.98711+03	\N	2026-09-21 22:01:39.11571+03	\N	t	2026-09-21 22:01:39.115606+03
fc8caed8-5b78-4620-bf67-d7675f9db015	Intro call	edited via timeline	2	1	\N	\N	f	b63ee261-855b-4957-9504-403e85275987	\N	\N	2026-09-21 22:00:59.884774+03	\N	2026-09-21 22:02:19.092516+03	\N	t	2026-09-21 22:02:19.092426+03
3cbc858e-ac06-4742-b3ba-70ab55f9b374	Intro call	Discussed scope and pricing	2	1	\N	\N	f	ab7a29d1-59cb-4c5b-a19b-55ad8740457f	\N	\N	2026-09-21 22:12:00.51867+03	\N	2026-09-21 23:50:16.843254+03	\N	t	2026-09-21 23:50:16.777919+03
\.


--
-- Data for Name: applications; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.applications ("Id", "Title", "Stage", "StageSemantic", "IsClosed", "Amount", "CurrencyCode", "Probability", "BeginDate", "CloseDate", "ContactId", "CompanyId", "AssignedToId", "CreatedAt", "CreatedById", "UpdatedAt", "UpdatedById", "IsDeleted", "DeletedAt", "MovedAt", "PreviousStageId") FROM stdin;
19ce3025-f563-48a5-93a8-d6c5c157a966	ERP rollout	5	1	t	48500.00	USD	40	2026-09-21 13:22:15.294221+03	2026-09-21 13:22:24.794323+03	84725442-436d-4fe1-bf77-50512ec32853	\N	\N	2026-09-21 13:22:15.380143+03	\N	2026-09-21 13:22:24.85931+03	\N	t	2026-09-21 13:22:24.858577+03	\N	\N
99cd913f-d2fd-4f23-9a1e-2e5cff128143	Website redesign	3	0	f	25000.00	USD	60	2026-09-21 13:42:36.751105+03	2026-09-28 13:42:36.751152+03	\N	\N	\N	2026-09-21 13:42:36.840974+03	\N	2026-09-21 13:45:04.405567+03	\N	t	2026-09-21 13:45:04.387616+03	\N	\N
5c7fcfc8-22fb-48be-90ef-b7c3baed50aa	Stage rules test	5	1	t	1000.00	USD	10	2026-09-21 15:38:25.292415+03	2026-09-21 15:38:35.781217+03	\N	\N	\N	2026-09-21 15:38:25.436946+03	\N	2026-09-21 15:40:59.485804+03	\N	t	2026-09-21 15:40:59.485688+03	2026-09-21 15:38:35.781217+03	0
3d743f49-1085-4de3-8f2f-82a2fcd42267	ERP rollout phase 2	2	0	f	62000.00	USD	60	2026-09-21 16:41:29.377918+03	2026-09-28 16:41:29.37795+03	\N	\N	\N	2026-09-21 16:41:29.443139+03	\N	2026-09-21 16:43:11.222975+03	\N	t	2026-09-21 16:43:11.222789+03	2026-09-21 16:42:52.569117+03	1
111aa027-fdf1-49a1-96ae-33b1afb0c239	Edited deal	1	0	f	7000.00	USD	60	2026-09-21 21:50:50.128283+03	2026-09-28 21:50:50.128305+03	\N	\N	\N	2026-09-21 21:50:50.190042+03	\N	2026-09-21 21:51:30.812692+03	\N	t	2026-09-21 21:51:30.811809+03	2026-09-21 21:50:50.655052+03	0
eafe96f7-a09d-41b9-94e4-a6ff6eaafa85	ERP rollout	0	0	f	48500.00	USD	40	2026-09-21 21:51:43.724135+03	2026-09-28 21:51:43.724135+03	e8d1b76d-83ec-416e-8fb8-e9caf1e81231	\N	\N	2026-09-21 21:51:43.724515+03	\N	2026-09-21 21:55:25.446503+03	\N	t	2026-09-21 21:55:25.446435+03	\N	\N
36a7dada-5cd7-4185-aced-ec5317ca8c9c	ERP rollout	0	0	f	48500.00	USD	40	2026-09-21 22:12:00.673387+03	2026-09-28 22:12:00.673388+03	ab7a29d1-59cb-4c5b-a19b-55ad8740457f	\N	\N	2026-09-21 22:12:00.674245+03	\N	2026-09-21 23:50:16.610549+03	\N	t	2026-09-21 23:50:16.520332+03	\N	\N
\.


--
-- Data for Name: companies; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.companies ("Id", "Name", "Industry", "Website", "Phone", "Email", "AddressCity", "AddressCountry", "AnnualRevenue", "EmployeeCount", "AssignedToId", "CreatedAt", "CreatedById", "UpdatedAt", "UpdatedById", "IsDeleted", "DeletedAt") FROM stdin;
676c0903-e5b3-4b0b-b610-23eff2844ba1	Nexus Systems LLC	Software	\N	\N	info@nexus.jo	\N	\N	\N	\N	\N	2026-09-21 16:41:19.870697+03	\N	2026-09-21 16:43:11.478515+03	\N	t	2026-09-21 16:43:11.478115+03
05c9a41b-4be0-4a5d-b1e3-7dab19f696f5	Nexus Systems	Software	nexus.jo	962791000000	info@nexus.jo	Amman	Jordan	\N	45	\N	2026-09-21 16:45:56.909101+03	\N	2026-09-21 16:46:34.95501+03	\N	t	2026-09-21 16:46:34.954953+03
8cd6b955-9a4f-4063-be56-87f1042c6e04	Petra Logistics	Transport	\N	\N	\N	Aqaba	\N	\N	120	\N	2026-09-21 16:45:56.997048+03	\N	2026-09-21 16:46:34.98948+03	\N	t	2026-09-21 16:46:34.989422+03
b274bc00-f54e-472c-9378-3e915f58ae6b	Nexus Systems	Software	nexus.jo	962791000000	info@nexus.jo	Amman	\N	\N	45	\N	2026-09-21 16:53:46.969251+03	\N	2026-09-21 16:54:41.669807+03	\N	t	2026-09-21 16:54:41.669735+03
72ec03c7-abfd-4499-ad4e-3f12be449c49	Petra Logistics	Transport	\N	\N	\N	Aqaba	\N	\N	\N	\N	2026-09-21 16:53:47.456838+03	\N	2026-09-21 16:54:41.706139+03	\N	t	2026-09-21 16:54:41.705986+03
d02fb622-35e7-4e42-a98b-11fa729b1ed6	Nexus Systems	Software	\N	\N	\N	\N	\N	\N	\N	\N	2026-09-21 17:00:48.606599+03	\N	2026-09-21 17:02:09.288147+03	\N	t	2026-09-21 17:02:09.288044+03
448c346e-3ea5-438e-a82a-dc9dae81688c	Nexus Systems	\N	\N	\N	\N	\N	\N	\N	\N	\N	2026-09-21 17:05:19.925048+03	\N	2026-09-21 17:07:15.28832+03	\N	t	2026-09-21 17:07:15.288258+03
b129ad47-82da-4dbf-963d-6de56c92460a	Nexus Systems	\N	\N	\N	\N	\N	\N	\N	\N	\N	2026-09-21 21:29:39.439501+03	\N	2026-09-21 21:32:00.442641+03	\N	t	2026-09-21 21:32:00.4426+03
68b3a4d6-2003-4fa8-aedf-88748867485b	Nexus Systems	Software	\N	\N	\N	\N	\N	\N	\N	\N	2026-09-21 21:41:16.922043+03	\N	2026-09-21 21:43:23.43506+03	\N	t	2026-09-21 21:43:23.435009+03
6635d1c4-1897-4c7c-81fc-f90ff8d68764	TestCo Renamed	\N	\N	\N	\N	\N	\N	\N	\N	\N	2026-09-21 21:49:58.499743+03	\N	2026-09-21 21:51:30.912061+03	\N	t	2026-09-21 21:51:30.91122+03
fd051ae9-62cc-44f4-9f18-2b0786204484	Nexus Systems	Software	\N	\N	\N	Amman	\N	\N	\N	\N	2026-09-21 21:51:43.402652+03	\N	2026-09-21 21:55:25.558787+03	\N	t	2026-09-21 21:55:25.558744+03
11387a21-5f0c-401d-8236-71d327580b09	Nexus Systems	Software	nexus.jo	962791000000	info@nexus.jo	Amman	\N	\N	45	\N	2026-09-21 22:11:58.379461+03	\N	2026-09-21 23:50:16.999646+03	\N	t	2026-09-21 23:50:16.999391+03
659b581f-ae0f-480a-adcc-73f57278d291	Petra Logistics	Transport	\N	\N	\N	Aqaba	\N	\N	\N	\N	2026-09-21 22:11:58.629525+03	\N	2026-09-21 23:50:17.011806+03	\N	t	2026-09-21 23:50:17.011625+03
e14ec2a5-c290-482d-b4fc-7802811412d4	Abdul Latif Jameel Finance	\N	aljfinance.com	+966546607565	Zahranif@aljfs.com	Jeddah	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.006204+03	\N	2026-09-21 23:57:42.069683+03	\N	f	\N
fa69381e-6b24-4348-9eb0-bf55270d6c3e	ADD Accounting Software	\N	addaccounting.com	+966530932037	sales@addaccounting.com	\N	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.082409+03	\N	2026-09-21 23:57:42.080333+03	\N	f	\N
5ca5889f-8bdd-4043-84e7-1969c58bd0c8	Al Adaa Al Raida	\N	alada-r.com.sa	+966554115905	Muath@alada-r.com.sa	\N	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.111843+03	\N	2026-09-21 23:57:42.091183+03	\N	f	\N
1b37eb7f-d714-4f5f-8c53-c3662fb266af	Bank Albilad	\N	bankalbilad.com.sa	+966598777770	asaadalshahrani2@bankalbilad.com.sa	\N	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.125313+03	\N	2026-09-21 23:57:42.101522+03	\N	f	\N
a1e294ba-f4ca-45c9-8b12-48272ed11b33	ClearGrid	\N	www.cleargrid.co	+971505277980	jamil@cleargrid.co	\N	UAE	\N	\N	\N	2026-09-21 23:51:40.051706+03	\N	2026-09-21 23:57:42.112869+03	\N	f	\N
1ab3f145-7e58-4d52-ae48-f70219f60c37	ePAiD	\N	ePaid.sa	+966569191913	Ahmad.fayyadh@ePaid.sa	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.073255+03	\N	2026-09-21 23:57:42.124199+03	\N	f	\N
ca962ce8-7af7-4ef0-aa19-ff6d8e2e3f3b	erad	\N	erad.co	+966559276578	shady@erad.co	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:39.994686+03	\N	2026-09-21 23:57:42.135397+03	\N	f	\N
14bb8bc0-6278-4a53-b3ef-e48556399fd4	Gulf International Bank	\N	www.gib.com	+966532668733	abdulrahman.aldayel@gib.com	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.034026+03	\N	2026-09-21 23:57:42.145332+03	\N	f	\N
62576646-51e4-4e1f-ba83-d9983af2e67b	Infosys	\N	www.infosys.com	+971565089372	ganesh_premsankar@infosys.com	Dubai	UAE	\N	\N	\N	2026-09-21 23:51:39.936892+03	\N	2026-09-21 23:57:42.155664+03	\N	f	\N
66c80106-a2ab-428a-b61e-61b9cfd9633a	Khazna	\N	khazna.app	+966557998222	s.alotaibi@khazna.app	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.102149+03	\N	2026-09-21 23:57:42.164265+03	\N	f	\N
b13122f5-a9f9-40da-96b1-9830e53a76b1	LEAN	\N	leantech.me	+966581881852	sami.aburashid@leantech.me	\N	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.016579+03	\N	2026-09-21 23:57:42.17438+03	\N	f	\N
93a58313-a8e2-4af0-8149-beb6429f9f34	Sardine.ai	\N	sardine.ai	+971585281192	max.vanderkolk@sardine.ai	\N	UAE	\N	\N	\N	2026-09-21 23:51:40.062569+03	\N	2026-09-21 23:57:42.18386+03	\N	f	\N
e019b312-1a95-4aa0-9cea-32ea0bdb0c7e	Sufrah	\N	www.sufrah.sa	+966562897103	rashad@sufrah.sa	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.092942+03	\N	2026-09-21 23:57:42.191816+03	\N	f	\N
934ce48e-19b3-407b-ae28-ed23de9ccf2d	TRENT	\N	trent.sa	+966541536051	Abdulkareem@trent.sa	\N	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.025789+03	\N	2026-09-21 23:57:42.200775+03	\N	f	\N
cdaad07f-a43f-43b1-af4e-881fca14593b	WUKAD	\N	www.wukad.com	+966552873666	m.elnaggar@wukad.com	\N	Saudi Arabia	\N	\N	\N	2026-09-21 23:51:40.042988+03	\N	2026-09-21 23:57:42.213307+03	\N	f	\N
1ae768d6-445c-4dad-9a13-e6e5950eaa9f	Mani	\N	Maniksa.com	+201061994965	mohamed.eid@maniksa.com	\N	Egypt	\N	\N	\N	2026-09-22 00:03:41.907237+03	\N	\N	\N	f	\N
1b21b418-2269-456b-800c-3fd2ba8a7092	SAHAM Plus Company	\N	sahammedical.com	+966596559666	hani.alaidaros@sahammedical.com	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-22 00:03:41.929328+03	\N	\N	\N	f	\N
6741636a-9d74-4950-9016-823439c08c2c	erad	\N	erad.co	+966553930018	motasem@erad.co	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-22 00:03:41.937573+03	\N	\N	\N	f	\N
e7f9c510-0e5b-4ca3-9507-8b007388442e	Incorta	\N	incorta.com	+16502180101	mohamed.sabry@incorta.com	\N	USA	\N	\N	\N	2026-09-22 00:03:41.945526+03	\N	\N	\N	f	\N
b31fe765-c598-4e64-9b27-dcdb30e0a9e4	ProConsult International	\N	www.proconsulti.com	+971503445308	ceo@proconsulti.com	Dubai	UAE	\N	\N	\N	2026-09-22 00:03:41.953766+03	\N	\N	\N	f	\N
48c22967-5596-4ebf-9f7a-90afd26aadf1	Commercial Bank of Ceylon PLC	\N	www.combank.lk	+966544564253	shehan_fernando@combank.net	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:03:41.960395+03	\N	\N	\N	f	\N
3cc42765-1408-4bbc-82c1-3b7905d195f0	KamelPay	\N	www.kamelpay.com	+971561361134	rubab.ashraf@kamelpay.com	Dubai	UAE	\N	\N	\N	2026-09-22 00:03:41.967252+03	\N	\N	\N	f	\N
fe671919-e811-4297-a8ae-0baf7d0eb585	ARIB	\N	www.arib.com.sa	+966543888447	Majmohassan@arib.com.sa	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:03:41.974379+03	\N	\N	\N	f	\N
998442cb-ffc3-414b-8e6a-3b2479efa252	HEVN Inc.	Payments	gethevn.com	\N	p@gethevn.com	\N	\N	\N	\N	\N	2026-09-22 00:05:21.332303+03	\N	\N	\N	f	\N
c9fdfaf6-8dda-4b13-8ba4-5aa3e9214176	Tamara	\N	tamara.co	+966565760573	abdulaziz.alshahrani@tamara.co	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:11:14.49508+03	\N	\N	\N	f	\N
a3e742e0-2c40-44ad-ae19-bc3e05803c19	Nqoodlet	Fintech	nqoodlet.com	+966570096677	\N	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:16:09.781789+03	\N	\N	\N	f	\N
234fa1d5-6a46-45ed-9822-d1abe2a71e7d	NearPay	\N	www.nearpay.io	+966550138455	business@nearpay.io	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-22 00:17:08.175783+03	\N	\N	\N	f	\N
531c178a-4d3b-4da1-9482-c16e27667dc1	Aljazira Capital	\N	aljaziracapital.com.sa	+966112157000	f.almangour@aljaziracapital.com.sa	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:17:08.208933+03	\N	\N	\N	f	\N
1ba574d3-5f38-4625-9a89-bc84e34844ab	Signit	\N	signit.sa	+966563222239	nizar.abdulhadi@signit.sa	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:18:22.207385+03	\N	\N	\N	f	\N
0c4d6aa3-ab17-428d-8c3a-7ec7899030de	abwab.ai	Fintech	www.abwab.ai	+966534482544	mhashem@abwab.ai	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:20:10.27252+03	\N	\N	\N	f	\N
b2eecba3-a07d-4fe7-9edb-79abf332d4f1	GMR International Group Ltd.	Money Exchange - OMXP / TOWB / OMXE	omxp.com	+447412851000	sara.loren@omxp.com	London	United Kingdom	\N	\N	\N	2026-09-22 00:21:23.900788+03	\N	\N	\N	f	\N
ef2bd36c-86af-42a7-9afe-a89e9244f04f	ContentFlow	\N	contentflow.click	+966565119542	lodhikpsingh@contentflow.click	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:22:43.205294+03	\N	\N	\N	f	\N
b9000834-765a-45fa-8328-15056f7b04f9	Sorbet	Payments	mysorbet.io	+33609068645	maher@mysorbet.io	\N	France	\N	\N	\N	2026-09-22 00:24:13.199287+03	\N	\N	\N	f	\N
a04f1ae8-5d20-4c4e-b73c-3a838ba6799e	azakaw	Part of Awan & Partners group	azakaw.com	+971585996968	zohaib.awan@azakaw.com	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-22 00:25:23.164094+03	\N	\N	\N	f	\N
907a620b-724b-4458-ab02-fb5fb6874732	Comfi	Business Finance	comfi.ai	+971569953107	mo@comfi.ai	\N	United Arab Emirates	\N	\N	\N	2026-09-22 00:26:23.260288+03	\N	\N	\N	f	\N
d9cbea35-82cc-466e-a53d-4a6712338630	fina	\N	fina.sa	+966544276327	jawad.alramis@silqfi.com	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:27:17.87797+03	\N	\N	\N	f	\N
e547d878-9693-467f-b87a-ba85904539bd	Solidrange	Compliance	www.solidrange.com	+966550722030	info@solidrange.com	\N	Saudi Arabia	\N	\N	\N	2026-09-22 00:28:14.712807+03	\N	\N	\N	f	\N
6396f2d4-a235-4a8f-83d3-0961aa38f328	Zoho	Software - Toll Free 800 3011 222	zohocorp.com	8003011222	ksa-consultations@zohocorp.com	Riyadh	Saudi Arabia	\N	\N	\N	2026-09-22 00:29:12.600297+03	\N	\N	\N	f	\N
\.


--
-- Data for Name: contacts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.contacts ("Id", "FirstName", "LastName", "Email", "Phone", "Position", "CompanyId", "AssignedToId", "CreatedAt", "CreatedById", "UpdatedAt", "UpdatedById", "IsDeleted", "DeletedAt", "Status", "NextAction", "NextActionDate") FROM stdin;
84725442-436d-4fe1-bf77-50512ec32853	Mahmoud	Almomani	m@example.com	+962790000000	CTO	\N	\N	2026-09-21 13:22:15.046735+03	\N	2026-09-21 13:22:34.411497+03	\N	t	2026-09-21 13:22:34.411381+03	0	\N	\N
1ae5d7e3-91b1-437e-930b-d119dcd38253	Sara	Khalil	sara@acme.com	+962791111111	Procurement Lead	\N	\N	2026-09-21 13:42:36.587221+03	\N	2026-09-21 13:45:04.178955+03	\N	t	2026-09-21 13:45:04.153945+03	0	\N	\N
37dd8a62-1994-494a-8404-f6a83e720d13	Yousef	Ali	\N	\N	\N	\N	\N	2026-09-21 14:34:24.799102+03	\N	2026-09-21 14:34:55.684872+03	\N	t	2026-09-21 14:34:55.684791+03	0	\N	\N
8a9e5ea5-5765-4c66-b238-008297f2a26d	Rami	Saleh	rami@test.com	\N	\N	\N	\N	2026-09-21 15:38:46.178767+03	\N	2026-09-21 15:40:59.538692+03	\N	t	2026-09-21 15:40:59.538554+03	0	\N	\N
3b21d012-9087-4ae9-bd98-45962e49fafa	Tariq	Mansour	tariq@acme.jo	+962791234567	\N	\N	\N	2026-09-21 15:44:06.086068+03	\N	2026-09-21 15:45:04.238293+03	\N	t	2026-09-21 15:45:04.238212+03	5	\N	\N
c4ec86c5-91e1-4452-9776-b0664b6935f8	Noor	Aziz	\N	\N	\N	\N	\N	2026-09-21 15:44:16.339504+03	\N	2026-09-21 15:45:04.357056+03	\N	t	2026-09-21 15:45:04.356994+03	2	\N	\N
1dd6d931-cef2-42ff-a0a5-98d9a96fd9f4	Sami	Odeh	\N	\N	\N	\N	\N	2026-09-21 15:44:16.380669+03	\N	2026-09-21 15:45:04.475211+03	\N	t	2026-09-21 15:45:04.475155+03	2	\N	\N
baca2cd0-274d-41d1-910d-96a97f13e07f	Dana	Khoury	dana@nexus.jo	962795551234	Operations Manager	\N	\N	2026-09-21 15:49:00.610141+03	\N	2026-09-21 15:50:55.072019+03	\N	t	2026-09-21 15:50:55.071951+03	2	\N	\N
8572c1b8-df47-4dda-b98e-fc92be117486	Hala	Nasr	hala@ok.com	+962791112222	\N	\N	\N	2026-09-21 15:57:03.591365+03	\N	2026-09-21 16:00:54.155931+03	\N	t	2026-09-21 16:00:54.154864+03	4	\N	\N
7a7b30bf-5dca-4c9d-b822-c0c578fca6b3	Log	Test	log@t.com	\N	\N	\N	\N	2026-09-21 15:59:32.954838+03	\N	2026-09-21 16:00:54.293318+03	\N	t	2026-09-21 16:00:54.293086+03	4	\N	\N
4167818c-c1e5-4859-8177-ed0254b5795c	Final	Check	fc@t.com	\N	\N	\N	\N	2026-09-21 16:00:37.490712+03	\N	2026-09-21 16:00:54.431582+03	\N	t	2026-09-21 16:00:54.431498+03	4	\N	\N
0a78018c-3d9f-468e-bbb2-7bdf505edfd3	Dana	Khoury	dana@nexus.jo	962795551234	\N	\N	\N	2026-09-21 16:12:07.430533+03	\N	2026-09-21 16:13:36.139753+03	\N	t	2026-09-21 16:13:36.13853+03	2	Send the revised quote	2026-09-19 12:00:00+03
c9209567-d522-4ac7-b45f-558e92e4a4c1	Omar	Saleh	omar@globex.com	962791112222	\N	\N	\N	2026-09-21 16:12:07.581789+03	\N	2026-09-21 16:13:36.276826+03	\N	t	2026-09-21 16:13:36.276752+03	3	Follow up on contract signature	2026-09-21 12:00:00+03
93e0015b-532a-45a8-b103-01a23d7d264e	Lina	Hadid	lina@acme.jo	962793334444	\N	\N	\N	2026-09-21 16:12:07.650416+03	\N	2026-09-21 16:13:36.426405+03	\N	t	2026-09-21 16:13:36.426335+03	4	Quarterly check-in call	2026-09-25 12:00:00+03
d0aaf68d-eb83-458c-8765-ded6fbf22886	Sami	Nasr	sami@start.io	\N	\N	\N	\N	2026-09-21 16:12:07.737132+03	\N	2026-09-21 16:13:36.518848+03	\N	t	2026-09-21 16:13:36.518798+03	0	\N	\N
d0b6b01a-3d19-4292-9e93-1656b6442db1	Rana	Aziz	rana@old.co	\N	\N	\N	\N	2026-09-21 16:12:07.818207+03	\N	2026-09-21 16:13:36.614744+03	\N	t	2026-09-21 16:13:36.614691+03	5	\N	\N
e593aed2-0ee2-4f10-aa9c-439eb4fef445	Test	Search	\N	\N	\N	\N	\N	2026-09-21 16:12:23.343656+03	\N	2026-09-21 16:13:36.709723+03	\N	t	2026-09-21 16:13:36.709671+03	0	\N	\N
fdad195c-2fbd-4992-814d-5bc3acbb3245	Dana	Khoury-Said	dana@nexus.jo	\N	\N	676c0903-e5b3-4b0b-b610-23eff2844ba1	\N	2026-09-21 16:41:20.2248+03	\N	2026-09-21 16:43:11.345185+03	\N	t	2026-09-21 16:43:11.344776+03	3	\N	\N
63d04ed2-4773-4bbb-9398-c7fa71baea12	Omar	Saleh	omar@nexus.jo	962791112222	\N	\N	\N	2026-09-21 16:42:13.183528+03	\N	2026-09-21 16:43:11.379929+03	\N	t	2026-09-21 16:43:11.379866+03	0	\N	\N
6795a0d1-b829-4a99-be4f-19abc824325f	bahaa	Alomari	bahaa.alomari077@gmail.com	0776895381	\N	\N	\N	2026-09-21 16:45:05.213694+03	\N	2026-09-21 16:46:34.727532+03	\N	t	2026-09-21 16:46:34.727424+03	1	\N	\N
f1dd6ada-81cf-4065-b5bd-ecb400b60094	Dana	Khoury	dana@nexus.jo	\N	\N	05c9a41b-4be0-4a5d-b1e3-7dab19f696f5	\N	2026-09-21 16:45:57.05877+03	\N	2026-09-21 16:46:34.764212+03	\N	t	2026-09-21 16:46:34.764152+03	2	\N	\N
04fa1dfd-2c31-47d3-928b-4b89b5ade380	Omar	Saleh	omar@nexus.jo	\N	\N	05c9a41b-4be0-4a5d-b1e3-7dab19f696f5	\N	2026-09-21 16:45:57.093714+03	\N	2026-09-21 16:46:34.799947+03	\N	t	2026-09-21 16:46:34.799885+03	4	\N	\N
c8107f2e-51de-44eb-a51b-b27e2f73e6c8	Lina	Hadid	\N	\N	\N	\N	\N	2026-09-21 16:45:57.127966+03	\N	2026-09-21 16:46:34.835229+03	\N	t	2026-09-21 16:46:34.83517+03	0	\N	\N
63fc394e-2576-4d9c-acf9-04072e393851	DbProof	164805	proof@test.jo	\N	\N	\N	\N	2026-09-21 16:48:06.033803+03	\N	2026-09-21 16:48:54.71917+03	\N	t	2026-09-21 16:48:54.71834+03	4	تم التعديل من SQL مباشرة	2026-09-24 16:48:39.639153+03
4a5ed0a4-1c37-42ab-8c90-cc3152438e97	Audit	Test	\N	\N	\N	\N	\N	2026-09-21 16:50:46.028851+03	\N	2026-09-21 16:54:41.14446+03	\N	t	2026-09-21 16:54:41.144232+03	0	\N	\N
6a9c6b40-708d-4a52-b24f-05b742391424	Dana	Khoury	dana@nexus.jo	\N	CTO	b274bc00-f54e-472c-9378-3e915f58ae6b	\N	2026-09-21 16:53:47.310908+03	\N	2026-09-21 16:54:41.292513+03	\N	t	2026-09-21 16:54:41.292317+03	2	\N	\N
a8b2fea5-ef46-4838-8f3b-8c6247a524d5	Omar	Saleh	omar@nexus.jo	\N	CEO	b274bc00-f54e-472c-9378-3e915f58ae6b	\N	2026-09-21 16:53:47.373698+03	\N	2026-09-21 16:54:41.427578+03	\N	t	2026-09-21 16:54:41.427504+03	4	\N	\N
db758462-2d5b-4871-b204-62cd3e5b38bf	Rana	Aziz	\N	\N	\N	b274bc00-f54e-472c-9378-3e915f58ae6b	\N	2026-09-21 16:53:47.414974+03	\N	2026-09-21 16:54:41.524147+03	\N	t	2026-09-21 16:54:41.524079+03	0	\N	\N
0b62b6ec-40e6-4991-8303-0eff48e469e6	Rana	Aziz	\N	\N	\N	d02fb622-35e7-4e42-a98b-11fa729b1ed6	\N	2026-09-21 17:00:48.781467+03	\N	2026-09-21 17:02:08.91944+03	\N	t	2026-09-21 17:02:08.919255+03	0	\N	\N
fe9e1492-c9a9-4674-bbf1-04766c8219cd	Sami	Nasr	\N	\N	\N	\N	\N	2026-09-21 17:00:48.848049+03	\N	2026-09-21 17:02:08.961585+03	\N	t	2026-09-21 17:02:08.960773+03	1	\N	\N
02f8c696-9411-4af4-95e8-ebe2cd4cbd7e	Omar	Saleh	\N	\N	\N	d02fb622-35e7-4e42-a98b-11fa729b1ed6	\N	2026-09-21 17:00:48.884647+03	\N	2026-09-21 17:02:08.99731+03	\N	t	2026-09-21 17:02:08.99723+03	1	\N	\N
84cc1dba-79e5-43ee-a957-44c164ac7537	Dana	Khoury	\N	\N	\N	\N	\N	2026-09-21 17:00:48.961063+03	\N	2026-09-21 17:02:09.03393+03	\N	t	2026-09-21 17:02:09.033143+03	2	Send the revised quote	2026-09-19 12:00:00+03
dbb0602a-c8ec-4feb-ab12-9e731a14616b	Lina	Hadid	\N	\N	\N	\N	\N	2026-09-21 17:00:48.997818+03	\N	2026-09-21 17:02:09.068298+03	\N	t	2026-09-21 17:02:09.06822+03	3	Follow up on contract	2026-09-21 12:00:00+03
ed077828-2aa1-40f6-8216-90963544e949	Tariq	Mansour	\N	\N	\N	d02fb622-35e7-4e42-a98b-11fa729b1ed6	\N	2026-09-21 17:00:49.032691+03	\N	2026-09-21 17:02:09.104264+03	\N	t	2026-09-21 17:02:09.104189+03	4	\N	\N
765ea05c-84f0-4268-8c85-6ad5657da4e5	Noor	Jamal	\N	\N	\N	\N	\N	2026-09-21 17:00:49.06758+03	\N	2026-09-21 17:02:09.139037+03	\N	t	2026-09-21 17:02:09.138959+03	5	\N	\N
ef238bc3-b5b6-434f-846a-5f1fd9d8e35f	Rana	Aziz	\N	962791111111	\N	448c346e-3ea5-438e-a82a-dc9dae81688c	\N	2026-09-21 17:05:19.995527+03	\N	2026-09-21 17:07:14.563288+03	\N	t	2026-09-21 17:07:14.563211+03	0	\N	\N
578f8b67-d9f6-4536-a765-d5b9ea4bbf84	Sami	Nasr	\N	962792222222	\N	\N	\N	2026-09-21 17:05:20.035488+03	\N	2026-09-21 17:07:14.707133+03	\N	t	2026-09-21 17:07:14.707074+03	2	\N	\N
f4f5ba42-674a-4b1d-8035-457eb2dfc65c	Omar	Saleh	omar@nexus.jo	962793333333	\N	448c346e-3ea5-438e-a82a-dc9dae81688c	\N	2026-09-21 17:05:20.075955+03	\N	2026-09-21 17:07:14.79974+03	\N	t	2026-09-21 17:07:14.799671+03	1	\N	\N
e76550d7-2340-4170-a73f-406f4127f885	Dana	Khoury	dana@x.jo	962794444444	\N	\N	\N	2026-09-21 17:05:20.115093+03	\N	2026-09-21 17:07:14.892757+03	\N	t	2026-09-21 17:07:14.892695+03	2	Send the revised quote	2026-09-19 12:00:00+03
40ff9b6e-0997-4c91-a377-75f3820acea8	Lina	Hadid	\N	962795555555	\N	\N	\N	2026-09-21 17:05:20.15111+03	\N	2026-09-21 17:07:14.987133+03	\N	t	2026-09-21 17:07:14.987076+03	3	\N	\N
95a902b5-0c71-4433-b75a-d0cfa661c21f	Tariq	Mansour	\N	962796666666	\N	448c346e-3ea5-438e-a82a-dc9dae81688c	\N	2026-09-21 17:05:20.189089+03	\N	2026-09-21 17:07:15.08219+03	\N	t	2026-09-21 17:07:15.082138+03	4	\N	\N
268d2283-0a49-4c51-9c6f-af1ece204478	Noor	Jamal	\N	\N	\N	\N	\N	2026-09-21 17:05:20.229817+03	\N	2026-09-21 17:07:15.192433+03	\N	t	2026-09-21 17:07:15.192375+03	5	\N	\N
898c723b-be81-42e6-be0f-f35cc9dede7b	Future	Five	\N	962791111111	\N	\N	\N	2026-09-21 17:08:44.727215+03	\N	2026-09-21 17:09:45.837964+03	\N	t	2026-09-21 17:09:45.837896+03	2	Future task	2026-09-26 12:00:00+03
d98c3cc5-83a6-4521-92ba-9d2df18035cc	Overdue	Five	\N	962792222222	\N	\N	\N	2026-09-21 17:08:44.81158+03	\N	2026-09-21 17:09:45.96367+03	\N	t	2026-09-21 17:09:45.963611+03	2	Very overdue	2026-09-16 12:00:00+03
48d8997d-4126-4d6b-acfd-ee59d1f1e03f	NoAction	Zed	\N	962793333333	\N	\N	\N	2026-09-21 17:08:44.895246+03	\N	2026-09-21 17:09:46.06735+03	\N	t	2026-09-21 17:09:46.0673+03	2	\N	\N
4bbe083e-b3aa-48f0-8c69-e8c5ff4ceef5	Today	Due	\N	962794444444	\N	\N	\N	2026-09-21 17:08:44.962889+03	\N	2026-09-21 17:09:46.219664+03	\N	t	2026-09-21 17:09:46.219614+03	2	Due today	2026-09-21 12:00:00+03
136515ff-ac4d-446d-a5ff-0b9fc131f565	Overdue	Two	\N	962795555555	\N	\N	\N	2026-09-21 17:08:45.037794+03	\N	2026-09-21 17:09:46.345086+03	\N	t	2026-09-21 17:09:46.34504+03	2	Slightly overdue	2026-09-19 12:00:00+03
0a973f47-0975-498c-bc53-440ff665486b	Rana	Aziz	\N	962791111111	\N	\N	\N	2026-09-21 17:15:33.248409+03	\N	2026-09-21 17:16:29.647478+03	\N	t	2026-09-21 17:16:29.647442+03	0	\N	\N
57adf04f-d300-4435-9d2b-52bf8fb50920	Omar	Saleh	\N	962792222222	\N	\N	\N	2026-09-21 17:15:33.288583+03	\N	2026-09-21 17:16:29.681545+03	\N	t	2026-09-21 17:16:29.681508+03	1	\N	\N
84e7de9c-e0a6-4d19-bb1c-e6a841a210c1	Dana	Khoury	\N	962793333333	\N	\N	\N	2026-09-21 17:15:33.324658+03	\N	2026-09-21 17:16:29.717104+03	\N	t	2026-09-21 17:16:29.717069+03	2	\N	\N
6a4c5e72-d208-4356-94aa-fef934f6c171	Lina	Hadid	\N	962794444444	\N	\N	\N	2026-09-21 17:15:33.359865+03	\N	2026-09-21 17:16:29.750526+03	\N	t	2026-09-21 17:16:29.75049+03	3	\N	\N
641a1c06-67fc-4e6f-ac83-261fc06df387	Tariq	Mansour	\N	962795555555	\N	\N	\N	2026-09-21 17:15:33.393438+03	\N	2026-09-21 17:16:29.79074+03	\N	t	2026-09-21 17:16:29.790704+03	4	\N	\N
c6489465-3a8c-48d6-8135-7669038a63c2	Noor	Jamal	\N	\N	\N	\N	\N	2026-09-21 17:15:33.42789+03	\N	2026-09-21 17:16:29.826905+03	\N	t	2026-09-21 17:16:29.826869+03	5	\N	\N
c2ca18f0-9581-4bdc-94f4-75542ab23dae	Rana	Aziz	\N	962791111111	\N	b129ad47-82da-4dbf-963d-6de56c92460a	\N	2026-09-21 21:29:39.533927+03	\N	2026-09-21 21:32:00.177269+03	\N	t	2026-09-21 21:32:00.177234+03	0	\N	\N
92eb36e0-64a7-44ab-9189-a8fed4dc5fce	Omar	Saleh	\N	962792222222	\N	b129ad47-82da-4dbf-963d-6de56c92460a	\N	2026-09-21 21:29:39.594298+03	\N	2026-09-21 21:32:00.211362+03	\N	t	2026-09-21 21:32:00.211319+03	1	\N	\N
b92423df-5433-445b-b0f3-669028a822b1	Dana	Khoury	\N	962793333333	\N	\N	\N	2026-09-21 21:29:39.667309+03	\N	2026-09-21 21:32:00.246032+03	\N	t	2026-09-21 21:32:00.245985+03	2	Send the revised quote	2026-09-19 12:00:00+03
d2904ec2-2e01-498a-b86d-740c30645feb	Lina	Hadid	\N	962794444444	\N	\N	\N	2026-09-21 21:29:39.739062+03	\N	2026-09-21 21:32:00.279497+03	\N	t	2026-09-21 21:32:00.279458+03	3	\N	\N
7e2d3a16-60ae-47a5-8ded-c47b5e56136f	Tariq	Mansour	\N	962795555555	\N	b129ad47-82da-4dbf-963d-6de56c92460a	\N	2026-09-21 21:29:39.790268+03	\N	2026-09-21 21:32:00.313699+03	\N	t	2026-09-21 21:32:00.313664+03	4	\N	\N
6af3db43-119f-4414-b995-af90e47e6bca	Petra	Logistics	\N	962799998888	\N	\N	\N	2026-09-21 21:30:31.061066+03	\N	2026-09-21 21:32:00.348788+03	\N	t	2026-09-21 21:32:00.348754+03	0	\N	\N
657e9813-9f31-4d47-a1aa-25357f715e3c	Nexus	—	info@nexus.com	0791234567	\N	\N	\N	2026-09-21 21:35:58.759705+03	\N	2026-09-21 21:37:20.174737+03	\N	t	2026-09-21 21:37:20.149319+03	0	\N	\N
f069cec8-40ab-4bec-9cfb-624c7bffa637	Omar	Tech	omar@mail.com	0787654321	\N	\N	\N	2026-09-21 21:35:58.759743+03	\N	2026-09-21 21:37:20.261551+03	\N	t	2026-09-21 21:37:20.261467+03	0	\N	\N
6925cda3-a547-4edf-8a1a-91bd0c79911b	Nexus		info@nexus.com	0791234567	\N	\N	\N	2026-09-21 21:37:20.404368+03	\N	2026-09-21 21:37:28.214485+03	\N	t	2026-09-21 21:37:28.214368+03	0	\N	\N
c83c808e-79ad-4ef3-9f8b-4f6a706d2df3	Dana	Khoury	\N	962793333333	\N	\N	\N	2026-09-21 21:37:28.265959+03	\N	2026-09-21 21:38:26.326103+03	\N	t	2026-09-21 21:38:26.325994+03	2	\N	\N
50e10068-f9e4-48a8-baa8-b60b43ff1ece	Tariq	Mansour	\N	962795555555	\N	\N	\N	2026-09-21 21:37:28.312871+03	\N	2026-09-21 21:38:26.360193+03	\N	t	2026-09-21 21:38:26.36014+03	4	\N	\N
3065ca43-fce8-4a48-92a0-ea2df40a85e9	Omar	Tech	omar@mail.com	0787654321	\N	\N	\N	2026-09-21 21:37:58.676554+03	\N	2026-09-21 21:38:26.394368+03	\N	t	2026-09-21 21:38:26.394312+03	0	\N	\N
8caa9a15-b70f-44e1-af8d-d103cc3f8609	Petra	Logistics	ops@petra.jo	0795551234	\N	\N	\N	2026-09-21 21:37:58.676554+03	\N	2026-09-21 21:38:26.428836+03	\N	t	2026-09-21 21:38:26.428782+03	0	\N	\N
f372f7ea-7f8a-4632-877d-371ad4b0933f	Nexus		info@nexus.com	0791234567	\N	\N	\N	2026-09-21 21:37:58.676554+03	\N	2026-09-21 21:38:26.478154+03	\N	t	2026-09-21 21:38:26.4781+03	0	\N	\N
2b27a6e7-645f-4e9d-bd9f-19df69c4778b	bahaa		bahaa.alomari077@gmail.com	0776895381	\N	\N	\N	2026-09-21 21:40:55.877688+03	\N	2026-09-21 21:42:06.71003+03	\N	t	2026-09-21 21:42:06.709968+03	0	\N	\N
f94c0a17-4c17-447d-a139-3c196ad3afc6	Rana	Aziz	\N	962791111111	\N	68b3a4d6-2003-4fa8-aedf-88748867485b	\N	2026-09-21 21:41:16.99657+03	\N	2026-09-21 21:43:23.133253+03	\N	t	2026-09-21 21:43:23.13318+03	0	\N	\N
a61dc0c2-d1e2-44b7-87b8-36b57eb83ebc	Omar	Saleh	\N	962792222222	\N	\N	\N	2026-09-21 21:41:17.049245+03	\N	2026-09-21 21:43:23.167219+03	\N	t	2026-09-21 21:43:23.16717+03	1	\N	\N
a1147134-5a7d-4413-9213-97e623e63584	Dana	Khoury	\N	962793333333	\N	\N	\N	2026-09-21 21:41:17.132172+03	\N	2026-09-21 21:43:23.201539+03	\N	t	2026-09-21 21:43:23.200278+03	2	Send the revised quote	2026-09-19 12:00:00+03
4a8d5df5-e621-46b8-9456-462d2217f73b	Lina	Hadid	\N	962794444444	\N	\N	\N	2026-09-21 21:41:17.195642+03	\N	2026-09-21 21:43:23.235461+03	\N	t	2026-09-21 21:43:23.235348+03	3	\N	\N
21cca6ed-11bd-45a6-a62c-efe285fa4c43	Tariq	Mansour	\N	962795555555	\N	68b3a4d6-2003-4fa8-aedf-88748867485b	\N	2026-09-21 21:41:17.256589+03	\N	2026-09-21 21:43:23.271548+03	\N	t	2026-09-21 21:43:23.271495+03	4	\N	\N
7a375647-554c-40c2-accd-41d4cb205c5f	Noor	Jamal	\N	\N	\N	\N	\N	2026-09-21 21:41:17.320506+03	\N	2026-09-21 21:43:23.306435+03	\N	t	2026-09-21 21:43:23.306387+03	5	\N	\N
0c1b3561-fcf0-469b-8fb1-3a28d129ea66	Nexus	Systems	\N	\N	\N	\N	\N	2026-09-21 21:47:28.400207+03	\N	2026-09-21 21:48:30.806383+03	\N	t	2026-09-21 21:48:30.806327+03	4	\N	\N
3a2f1752-dad8-4ce1-8d60-2a9fc91b3c22	Petra	Logistics	ops@petra.jo	0795551234	\N	\N	\N	2026-09-21 21:48:05.832628+03	\N	2026-09-21 21:48:30.84199+03	\N	t	2026-09-21 21:48:30.841942+03	2	\N	\N
00e441c3-42f8-4876-be32-f4c9a2992b48	Aqaba	Ports	info@aqaba.jo	0796667777	\N	\N	\N	2026-09-21 21:48:05.832629+03	\N	2026-09-21 21:48:30.876271+03	\N	t	2026-09-21 21:48:30.876231+03	2	\N	\N
f8f9a79e-f128-48a9-ae62-6ed32d227069	Test	Renamed	\N	\N	\N	\N	\N	2026-09-21 21:50:11.984234+03	\N	2026-09-21 21:51:30.860464+03	\N	t	2026-09-21 21:51:30.86043+03	2	Call back	2026-09-25 12:00:00+03
5aad7a60-43e8-4534-b4bb-e5944666cf95	Bulk	One	b1@x.com	\N	\N	\N	\N	2026-09-21 21:50:12.818317+03	\N	2026-09-21 21:51:43.280077+03	\N	t	2026-09-21 21:51:43.280039+03	4	\N	\N
e8d1b76d-83ec-416e-8fb8-e9caf1e81231	Dana	Khoury	dana@nexus.jo	962793333333	\N	fd051ae9-62cc-44f4-9f18-2b0786204484	\N	2026-09-21 21:51:43.481065+03	\N	2026-09-21 21:55:25.14728+03	\N	t	2026-09-21 21:55:25.147239+03	2	Send revised quote	2026-09-19 12:00:00+03
18f2ea04-e3b0-410a-94e9-ebd335fd6772	Omar	Saleh	\N	962792222222	\N	fd051ae9-62cc-44f4-9f18-2b0786204484	\N	2026-09-21 21:51:43.531583+03	\N	2026-09-21 21:55:25.252523+03	\N	t	2026-09-21 21:55:25.252469+03	4	\N	\N
ad516d0f-0306-4164-9ee3-606194e575ee	Rana	Aziz	\N	962791111111	\N	\N	\N	2026-09-21 21:51:43.59302+03	\N	2026-09-21 21:55:25.348418+03	\N	t	2026-09-21 21:55:25.348384+03	0	\N	\N
b63ee261-855b-4957-9504-403e85275987	Edit	Test	\N	\N	\N	\N	\N	2026-09-21 22:00:59.762088+03	\N	2026-09-21 22:02:19.168327+03	\N	t	2026-09-21 22:02:19.168288+03	2	\N	\N
8cbf19ae-da64-49ad-b914-6f7663456789	Rana	Aziz	\N	962791111111	\N	11387a21-5f0c-401d-8236-71d327580b09	\N	2026-09-21 22:11:58.787838+03	\N	2026-09-21 23:50:16.898997+03	\N	t	2026-09-21 23:50:16.898765+03	0	\N	\N
2a2e8d08-ea93-4c71-8c1b-b62f7b8e5c73	Omar	Saleh	omar@x.jo	962792222222	\N	\N	\N	2026-09-21 22:11:59.015614+03	\N	2026-09-21 23:50:16.91292+03	\N	t	2026-09-21 23:50:16.912737+03	1	\N	\N
ab7a29d1-59cb-4c5b-a19b-55ad8740457f	Dana	Khoury	dana@nexus.jo	962793333333	CTO	11387a21-5f0c-401d-8236-71d327580b09	\N	2026-09-21 22:11:59.361898+03	\N	2026-09-21 23:50:16.922121+03	\N	t	2026-09-21 23:50:16.921931+03	2	Send the revised quote	2026-09-19 12:00:00+03
7f0fbf5e-3020-471f-8def-f7d7753eaf91	Lina	Hadid	\N	962794444444	\N	\N	\N	2026-09-21 22:11:59.593821+03	\N	2026-09-21 23:50:16.929724+03	\N	t	2026-09-21 23:50:16.929559+03	3	\N	\N
3fe41703-1e56-4d80-95ca-cad31767cfaa	Tariq	Mansour	\N	962795555555	\N	11387a21-5f0c-401d-8236-71d327580b09	\N	2026-09-21 22:11:59.849753+03	\N	2026-09-21 23:50:16.937082+03	\N	t	2026-09-21 23:50:16.936924+03	4	\N	\N
ccacd393-d1f3-485b-a5ac-d3d05c744be4	Noor	Jamal	\N	\N	\N	\N	\N	2026-09-21 22:12:00.092853+03	\N	2026-09-21 23:50:16.943125+03	\N	t	2026-09-21 23:50:16.942969+03	5	\N	\N
d1452a4d-a26f-48b1-a2d1-a51dea8d1c45	Ganesh	Premsankar	ganesh_premsankar@infosys.com	+971565089372	Senior Business Development Manager	62576646-51e4-4e1f-ba83-d9983af2e67b	\N	2026-09-21 23:51:39.981838+03	\N	2026-09-21 23:57:41.424773+03	\N	f	\N	0	\N	\N
c9f7f092-3d59-403e-b8ef-bfaf28526aa9	Shady	Zaki	shady@erad.co	+966559276578	Business Development Manager	ca962ce8-7af7-4ef0-aa19-ff6d8e2e3f3b	\N	2026-09-21 23:51:40.000333+03	\N	2026-09-21 23:57:41.687751+03	\N	f	\N	0	\N	\N
60e6ab3b-928a-4f3c-8b4b-4bd9f6e9ebbc	Fatmah	Alzahrani	Zahranif@aljfs.com	+966546607565	Products Manager	e14ec2a5-c290-482d-b4fc-7802811412d4	\N	2026-09-21 23:51:40.011502+03	\N	2026-09-21 23:57:41.710723+03	\N	f	\N	0	\N	\N
2254aa53-574e-4f02-b041-a0e822c6c430	Sami	Aburashid	sami.aburashid@leantech.me	+966581881852	Head of GTM - KSA	b13122f5-a9f9-40da-96b1-9830e53a76b1	\N	2026-09-21 23:51:40.020747+03	\N	2026-09-21 23:57:41.727405+03	\N	f	\N	0	\N	\N
1863a7a1-6f6d-4766-ac94-225c21aefb1e	Abdulkareem	Almazied	Abdulkareem@trent.sa	+966541536051	CTO	934ce48e-19b3-407b-ae28-ed23de9ccf2d	\N	2026-09-21 23:51:40.029912+03	\N	2026-09-21 23:57:41.743843+03	\N	f	\N	0	\N	\N
de4d31c2-369d-4e06-a78c-63a3acbffd4d	Abdulrahman	Al-Dayel	abdulrahman.aldayel@gib.com	+966532668733	Team Leader, Wholesale Banking Group	14bb8bc0-6278-4a53-b3ef-e48556399fd4	\N	2026-09-21 23:51:40.038784+03	\N	2026-09-21 23:57:41.803001+03	\N	f	\N	0	\N	\N
757e1201-d60b-4f44-95ce-445b3ac6337e	Mohamed	ElNaggar	m.elnaggar@wukad.com	+966552873666	Business Development Director	cdaad07f-a43f-43b1-af4e-881fca14593b	\N	2026-09-21 23:51:40.047285+03	\N	2026-09-21 23:57:41.821876+03	\N	f	\N	0	\N	\N
ecc48fba-f3d7-4f6a-a371-d2bb5cfc1d3f	Jammil	Khammu	jamil@cleargrid.co	+971505277980	Commercial Director	a1e294ba-f4ca-45c9-8b12-48272ed11b33	\N	2026-09-21 23:51:40.056983+03	\N	2026-09-21 23:57:41.85294+03	\N	f	\N	0	\N	\N
b066d06f-e28a-47ee-9c26-d1b4922bcda8	Max	van der Kolk	max.vanderkolk@sardine.ai	+971585281192	VP Sales, Middle East	93a58313-a8e2-4af0-8149-beb6429f9f34	\N	2026-09-21 23:51:40.067603+03	\N	2026-09-21 23:57:41.869285+03	\N	f	\N	0	\N	\N
f0ba8761-7150-42fb-ae40-d5c74ecb0162	Ahmad	Fayyadh	Ahmad.fayyadh@ePaid.sa	+966569191913	Business Development Manager	1ab3f145-7e58-4d52-ae48-f70219f60c37	\N	2026-09-21 23:51:40.07785+03	\N	2026-09-21 23:57:41.882639+03	\N	f	\N	0	\N	\N
20fa72bb-812a-43aa-a48e-b4fbc6c54640	ADD	Sales	sales@addaccounting.com	+966530932037	Sales	fa69381e-6b24-4348-9eb0-bf55270d6c3e	\N	2026-09-21 23:51:40.088018+03	\N	2026-09-21 23:57:41.893486+03	\N	f	\N	0	\N	\N
c168e153-336b-412a-bb96-f849db690eba	Rashad	Al-Zamar	rashad@sufrah.sa	+966562897103	Founder & CEO	e019b312-1a95-4aa0-9cea-32ea0bdb0c7e	\N	2026-09-21 23:51:40.097111+03	\N	2026-09-21 23:57:41.905604+03	\N	f	\N	0	\N	\N
074dc5b3-cc37-4cfd-bfdc-f128fb12eab8	Sultan	Mutlaq Alotaibi	s.alotaibi@khazna.app	+966557998222	Sales Executive	66c80106-a2ab-428a-b61e-61b9cfd9633a	\N	2026-09-21 23:51:40.106699+03	\N	2026-09-21 23:57:41.917254+03	\N	f	\N	0	\N	\N
fe6e4ab6-cb9e-4faa-a08d-fee96d1f81ad	Muath	Al Dawood	Muath@alada-r.com.sa	+966554115905	Account Manager	5ca5889f-8bdd-4043-84e7-1969c58bd0c8	\N	2026-09-21 23:51:40.117222+03	\N	2026-09-21 23:57:41.929664+03	\N	f	\N	0	\N	\N
a0cf7d3d-5ed3-4d22-a5ea-e5c7e81b6dfd	Abdulaziz	Saad Alshahrani	asaadalshahrani2@bankalbilad.com.sa	+966598777770	SME Partnership Development Manager	1b37eb7f-d714-4f5f-8c53-c3662fb266af	\N	2026-09-21 23:51:40.129763+03	\N	2026-09-21 23:57:41.944831+03	\N	f	\N	0	\N	\N
40fa0ecf-59cd-4edd-ac4d-9525eea87d92	Plus	Test	plus@test.local	+962790000000	\N	\N	\N	2026-09-21 23:58:12.674203+03	\N	2026-09-21 23:58:12.829744+03	\N	t	2026-09-21 23:58:12.829011+03	0	\N	\N
0205f2dc-b094-4663-83a6-175cf3c23267	Mohamed	Eid	mohamed.eid@maniksa.com	+201061994965	Sr. Manager Business Development	1ae768d6-445c-4dad-9a13-e6e5950eaa9f	\N	2026-09-22 00:03:41.922131+03	\N	\N	\N	f	\N	0	\N	\N
74c06449-f7a0-4845-b786-a7d201a3aea3	Hani	Al Aidaros	hani.alaidaros@sahammedical.com	+966596559666	General Manager	1b21b418-2269-456b-800c-3fd2ba8a7092	\N	2026-09-22 00:03:41.933554+03	\N	\N	\N	f	\N	0	\N	\N
f0c8ce92-52bf-436d-8ac2-bec6ea4df21f	Motasem	Habaireeh	motasem@erad.co	+966553930018	Senior Business Development Manager	6741636a-9d74-4950-9016-823439c08c2c	\N	2026-09-22 00:03:41.941265+03	\N	\N	\N	f	\N	0	\N	\N
8d755722-07d2-4f19-ab3d-b3303902618b	Mohamed	Sabry	mohamed.sabry@incorta.com	+16502180101	EMEA General Manager	e7f9c510-0e5b-4ca3-9507-8b007388442e	\N	2026-09-22 00:03:41.950033+03	\N	\N	\N	f	\N	0	\N	\N
983333bf-9832-4dc2-a076-7176b5118394	Umar	Daraz	ceo@proconsulti.com	+971503445308	Chief Executive Officer	b31fe765-c598-4e64-9b27-dcdb30e0a9e4	\N	2026-09-22 00:03:41.956988+03	\N	\N	\N	f	\N	0	\N	\N
bb1b3ff4-532d-4747-9352-2e4174a6e3d1	Shehan	Fernando	shehan_fernando@combank.net	+966544564253	Business Promotion Officer	48c22967-5596-4ebf-9f7a-90afd26aadf1	\N	2026-09-22 00:03:41.963607+03	\N	\N	\N	f	\N	0	\N	\N
f7fb26d5-0fc7-4af1-a2e9-f485bb289ae3	Rubab	Ashraf	rubab.ashraf@kamelpay.com	+971561361134	Business Development Manager	3cc42765-1408-4bbc-82c1-3b7905d195f0	\N	2026-09-22 00:03:41.970636+03	\N	\N	\N	f	\N	0	\N	\N
7a432365-c2f6-4463-86ea-2415b03983b4	Majid	Almohassan	Majmohassan@arib.com.sa	+966543888447	Business Development Manager	fe671919-e811-4297-a8ae-0baf7d0eb585	\N	2026-09-22 00:03:41.97795+03	\N	\N	\N	f	\N	0	\N	\N
76803f06-7f40-484d-a7e4-ca60db0978d9	HEVN	Payments	p@gethevn.com	\N	\N	998442cb-ffc3-414b-8e6a-3b2479efa252	\N	2026-09-22 00:05:21.342177+03	\N	\N	\N	f	\N	0	\N	\N
7b06671f-a9ba-4425-aea2-e591da649048	Rel	Check	rel@test.local	\N	\N	e14ec2a5-c290-482d-b4fc-7802811412d4	\N	2026-09-22 00:06:44.372415+03	\N	2026-09-22 00:06:44.712516+03	\N	t	2026-09-22 00:06:44.712101+03	0	\N	\N
9e5f143a-a91d-4729-956e-5332b1b9a881	Abdulaziz	Alshahrani	abdulaziz.alshahrani@tamara.co	+966565760573	Vertical Head - Health & Wellness	c9fdfaf6-8dda-4b13-8ba4-5aa3e9214176	\N	2026-09-22 00:11:14.510545+03	\N	\N	\N	f	\N	0	\N	\N
d046474f-f336-47aa-ad36-2b1a703e078e	Nqoodlet		\N	+966570096677	\N	a3e742e0-2c40-44ad-ae19-bc3e05803c19	\N	2026-09-22 00:16:09.789549+03	\N	\N	\N	f	\N	0	\N	\N
5d751338-98d3-4c35-9829-d35680d097f8	NearPay		business@nearpay.io	+966550138455	\N	234fa1d5-6a46-45ed-9822-d1abe2a71e7d	\N	2026-09-22 00:17:08.191294+03	\N	\N	\N	f	\N	0	\N	\N
a07089db-c0e6-4727-91ff-40bdf11358cf	Faisal	Almangour	f.almangour@aljaziracapital.com.sa	+966555274527	Digital Customer Service Analyst - Securities Group	531c178a-4d3b-4da1-9482-c16e27667dc1	\N	2026-09-22 00:17:08.21828+03	\N	\N	\N	f	\N	0	\N	\N
9e0566c9-eda2-4aeb-b0a3-a4d65d69f2f6	Nizar	Abdulhadi	nizar.abdulhadi@signit.sa	+966563222239	Regional Channel Director	1ba574d3-5f38-4625-9a89-bc84e34844ab	\N	2026-09-22 00:18:22.223225+03	\N	\N	\N	f	\N	0	\N	\N
a10a84c2-f7a7-4881-b916-0e2b82ba76d3	Mohammad	Hashem	mhashem@abwab.ai	+966534482544	Co-founder & CBO	0c4d6aa3-ab17-428d-8c3a-7ec7899030de	\N	2026-09-22 00:20:10.28225+03	\N	\N	\N	f	\N	0	\N	\N
712ab756-4e05-4326-81aa-dfe48dc9822d	Sara	Loren	sara.loren@omxp.com	+447412851000	Mena Region Manager	b2eecba3-a07d-4fe7-9edb-79abf332d4f1	\N	2026-09-22 00:21:23.910539+03	\N	\N	\N	f	\N	0	\N	\N
1c997824-1a94-4e00-ab4b-1c253195f2c9	K P	Singh	lodhikpsingh@contentflow.click	+966565119542	Co-founder & CTO	ef2bd36c-86af-42a7-9afe-a89e9244f04f	\N	2026-09-22 00:22:43.219114+03	\N	\N	\N	f	\N	0	\N	\N
48eff0b1-2f66-4d5c-a20a-40d1de125a41	Maher	Ayari	maher@mysorbet.io	+33609068645	CEO	b9000834-765a-45fa-8328-15056f7b04f9	\N	2026-09-22 00:24:13.211194+03	\N	\N	\N	f	\N	0	\N	\N
18c9cdd0-8981-41d8-a40b-397a9c8849b2	Zohaib	Awan	zohaib.awan@azakaw.com	+971585996968	Group Director, Business Development	a04f1ae8-5d20-4c4e-b73c-3a838ba6799e	\N	2026-09-22 00:25:23.173676+03	\N	\N	\N	f	\N	0	\N	\N
7c16c912-c619-4787-aa2d-38ba41a206e8	Mo	Gharbieh	mo@comfi.ai	+971569953107	Business Development Manager - Dealer Finance	907a620b-724b-4458-ab02-fb5fb6874732	\N	2026-09-22 00:26:23.27562+03	\N	\N	\N	f	\N	0	\N	\N
c117e800-ea14-4597-9c49-c398fdc932cb	Jawad	Alramis	jawad.alramis@silqfi.com	+966544276327	Senior Business Development Specialist	d9cbea35-82cc-466e-a53d-4a6712338630	\N	2026-09-22 00:27:17.889217+03	\N	\N	\N	f	\N	0	\N	\N
9a7d4b0c-049f-4057-a6a3-02aae4e888b3	Solidrange		info@solidrange.com	+966550722030	\N	e547d878-9693-467f-b87a-ba85904539bd	\N	2026-09-22 00:28:14.728042+03	\N	\N	\N	f	\N	0	\N	\N
63aeeaa1-d0d5-4179-9392-33817a1d7f85	Zoho	KSA	ksa-consultations@zohocorp.com	8003011222	\N	6396f2d4-a235-4a8f-83d3-0961aa38f328	\N	2026-09-22 00:29:12.608294+03	\N	\N	\N	f	\N	0	\N	\N
\.


--
-- Data for Name: messages; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.messages ("Id", "Channel", "Direction", "Subject", "Body", "SentAt", "IsRead", "ContactId", "UserId", "CreatedAt", "CreatedById", "UpdatedAt", "UpdatedById", "IsDeleted", "DeletedAt", "AttemptCount", "FailureReason", "LastAttemptAt", "ProviderMessageId", "Recipient", "Status") FROM stdin;
82174ef3-636a-44e7-8ca2-d437a046f291	2	1	\N	mlm mn	2026-09-21 21:41:14.22731+03	f	2b27a6e7-645f-4e9d-bd9f-19df69c4778b	\N	2026-09-21 21:41:14.264021+03	\N	2026-09-21 21:41:15.350541+03	\N	f	\N	1	WhatsApp API 400: {"error":{"message":"Unsupported post request. Object with ID 'messages' does not exist, cannot be loaded due to missing permissions, or does not support this operation. Please read the Graph API docu	2026-09-21 21:41:14.272465+03	\N	0776895381	4
85e9411f-a454-4033-a578-e98e28e3bcfe	0	1	yvgvg	bhgbhg	2026-09-21 21:41:25.867703+03	f	2b27a6e7-645f-4e9d-bd9f-19df69c4778b	\N	2026-09-21 21:41:25.869466+03	\N	2026-09-21 21:41:30.273836+03	\N	f	\N	1	SMTP error: No connection could be made because the target machine actively refused it.	2026-09-21 21:41:25.876073+03	\N	bahaa.alomari077@gmail.com	4
54aae240-6505-4de4-b81b-74e4689e6eff	0	1	s	b	2026-09-21 21:50:25.627071+03	f	f8f9a79e-f128-48a9-ae62-6ed32d227069	\N	2026-09-21 21:50:25.627392+03	\N	2026-09-21 21:50:29.722652+03	\N	f	\N	1	SMTP error: No connection could be made because the target machine actively refused it.	2026-09-21 21:50:25.630268+03	\N	x@y.com	4
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users ("Id", "FirstName", "LastName", "Email", "PasswordHash", "Role", "IsActive", "CreatedAt", "CreatedById", "UpdatedAt", "UpdatedById", "IsDeleted", "DeletedAt") FROM stdin;
\.


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: activities PK_activities; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.activities
    ADD CONSTRAINT "PK_activities" PRIMARY KEY ("Id");


--
-- Name: applications PK_applications; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.applications
    ADD CONSTRAINT "PK_applications" PRIMARY KEY ("Id");


--
-- Name: companies PK_companies; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT "PK_companies" PRIMARY KEY ("Id");


--
-- Name: contacts PK_contacts; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.contacts
    ADD CONSTRAINT "PK_contacts" PRIMARY KEY ("Id");


--
-- Name: messages PK_messages; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT "PK_messages" PRIMARY KEY ("Id");


--
-- Name: users PK_users; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT "PK_users" PRIMARY KEY ("Id");


--
-- Name: IX_activities_ApplicationId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_activities_ApplicationId" ON public.activities USING btree ("ApplicationId");


--
-- Name: IX_activities_AssignedToId_Deadline_IsCompleted; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_activities_AssignedToId_Deadline_IsCompleted" ON public.activities USING btree ("AssignedToId", "Deadline", "IsCompleted");


--
-- Name: IX_activities_ContactId_CreatedAt; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_activities_ContactId_CreatedAt" ON public.activities USING btree ("ContactId", "CreatedAt");


--
-- Name: IX_applications_AssignedToId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_applications_AssignedToId" ON public.applications USING btree ("AssignedToId");


--
-- Name: IX_applications_CompanyId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_applications_CompanyId" ON public.applications USING btree ("CompanyId");


--
-- Name: IX_applications_ContactId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_applications_ContactId" ON public.applications USING btree ("ContactId");


--
-- Name: IX_applications_Stage_IsClosed; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_applications_Stage_IsClosed" ON public.applications USING btree ("Stage", "IsClosed");


--
-- Name: IX_companies_AssignedToId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_companies_AssignedToId" ON public.companies USING btree ("AssignedToId");


--
-- Name: IX_companies_Name; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_companies_Name" ON public.companies USING btree ("Name");


--
-- Name: IX_contacts_AssignedToId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_contacts_AssignedToId" ON public.contacts USING btree ("AssignedToId");


--
-- Name: IX_contacts_CompanyId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_contacts_CompanyId" ON public.contacts USING btree ("CompanyId");


--
-- Name: IX_contacts_Email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_contacts_Email" ON public.contacts USING btree ("Email");


--
-- Name: IX_contacts_NextActionDate; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_contacts_NextActionDate" ON public.contacts USING btree ("NextActionDate");


--
-- Name: IX_contacts_Status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_contacts_Status" ON public.contacts USING btree ("Status");


--
-- Name: IX_messages_Channel; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_messages_Channel" ON public.messages USING btree ("Channel");


--
-- Name: IX_messages_ContactId_SentAt; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_messages_ContactId_SentAt" ON public.messages USING btree ("ContactId", "SentAt");


--
-- Name: IX_messages_ProviderMessageId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_messages_ProviderMessageId" ON public.messages USING btree ("ProviderMessageId");


--
-- Name: IX_messages_Status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_messages_Status" ON public.messages USING btree ("Status");


--
-- Name: IX_messages_UserId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_messages_UserId" ON public.messages USING btree ("UserId");


--
-- Name: IX_users_Email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX "IX_users_Email" ON public.users USING btree ("Email");


--
-- Name: activities FK_activities_applications_ApplicationId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.activities
    ADD CONSTRAINT "FK_activities_applications_ApplicationId" FOREIGN KEY ("ApplicationId") REFERENCES public.applications("Id") ON DELETE SET NULL;


--
-- Name: activities FK_activities_contacts_ContactId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.activities
    ADD CONSTRAINT "FK_activities_contacts_ContactId" FOREIGN KEY ("ContactId") REFERENCES public.contacts("Id") ON DELETE CASCADE;


--
-- Name: activities FK_activities_users_AssignedToId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.activities
    ADD CONSTRAINT "FK_activities_users_AssignedToId" FOREIGN KEY ("AssignedToId") REFERENCES public.users("Id") ON DELETE RESTRICT;


--
-- Name: applications FK_applications_companies_CompanyId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.applications
    ADD CONSTRAINT "FK_applications_companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES public.companies("Id") ON DELETE SET NULL;


--
-- Name: applications FK_applications_contacts_ContactId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.applications
    ADD CONSTRAINT "FK_applications_contacts_ContactId" FOREIGN KEY ("ContactId") REFERENCES public.contacts("Id") ON DELETE SET NULL;


--
-- Name: applications FK_applications_users_AssignedToId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.applications
    ADD CONSTRAINT "FK_applications_users_AssignedToId" FOREIGN KEY ("AssignedToId") REFERENCES public.users("Id") ON DELETE RESTRICT;


--
-- Name: companies FK_companies_users_AssignedToId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.companies
    ADD CONSTRAINT "FK_companies_users_AssignedToId" FOREIGN KEY ("AssignedToId") REFERENCES public.users("Id") ON DELETE RESTRICT;


--
-- Name: contacts FK_contacts_companies_CompanyId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.contacts
    ADD CONSTRAINT "FK_contacts_companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES public.companies("Id") ON DELETE SET NULL;


--
-- Name: contacts FK_contacts_users_AssignedToId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.contacts
    ADD CONSTRAINT "FK_contacts_users_AssignedToId" FOREIGN KEY ("AssignedToId") REFERENCES public.users("Id") ON DELETE RESTRICT;


--
-- Name: messages FK_messages_contacts_ContactId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT "FK_messages_contacts_ContactId" FOREIGN KEY ("ContactId") REFERENCES public.contacts("Id") ON DELETE CASCADE;


--
-- Name: messages FK_messages_users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT "FK_messages_users_UserId" FOREIGN KEY ("UserId") REFERENCES public.users("Id") ON DELETE SET NULL;


--
-- PostgreSQL database dump complete
--

\unrestrict LU8MialxrS66zJaTUEIaYaaALDOxEAX0VpSQQkjx0MgVEYfrCOp5hYXP6YmPJRs

