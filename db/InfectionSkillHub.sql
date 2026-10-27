--
-- PostgreSQL database dump
--

\restrict we5By7mSiMzbW92NFD8EoeedjwArNIrSgYXvpM2ImLdNhRKEKaoqj7kzygJApXk

-- Dumped from database version 16.15 (Ubuntu 16.15-0ubuntu0.24.04.1)
-- Dumped by pg_dump version 16.15 (Ubuntu 16.15-0ubuntu0.24.04.1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE IF EXISTS ONLY public.workshops DROP CONSTRAINT IF EXISTS "FK_workshops_instructors_instructor_id";
ALTER TABLE IF EXISTS ONLY public.participants DROP CONSTRAINT IF EXISTS "FK_participants_AspNetUsers_user_id";
ALTER TABLE IF EXISTS ONLY public.instructors DROP CONSTRAINT IF EXISTS "FK_instructors_AspNetUsers_user_id";
ALTER TABLE IF EXISTS ONLY public.enrollments DROP CONSTRAINT IF EXISTS "FK_enrollments_workshops_workshop_id";
ALTER TABLE IF EXISTS ONLY public.enrollments DROP CONSTRAINT IF EXISTS "FK_enrollments_participants_participant_id";
ALTER TABLE IF EXISTS ONLY public.enrollments DROP CONSTRAINT IF EXISTS "FK_enrollments_AspNetUsers_cancelled_by_user_id";
ALTER TABLE IF EXISTS ONLY public."AspNetUserTokens" DROP CONSTRAINT IF EXISTS "FK_AspNetUserTokens_AspNetUsers_UserId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserRoles" DROP CONSTRAINT IF EXISTS "FK_AspNetUserRoles_AspNetUsers_UserId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserRoles" DROP CONSTRAINT IF EXISTS "FK_AspNetUserRoles_AspNetRoles_RoleId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserLogins" DROP CONSTRAINT IF EXISTS "FK_AspNetUserLogins_AspNetUsers_UserId";
ALTER TABLE IF EXISTS ONLY public."AspNetUserClaims" DROP CONSTRAINT IF EXISTS "FK_AspNetUserClaims_AspNetUsers_UserId";
ALTER TABLE IF EXISTS ONLY public."AspNetRoleClaims" DROP CONSTRAINT IF EXISTS "FK_AspNetRoleClaims_AspNetRoles_RoleId";
DROP INDEX IF EXISTS public.ux_enrollments_active_participant_workshop;
DROP INDEX IF EXISTS public."UserNameIndex";
DROP INDEX IF EXISTS public."RoleNameIndex";
DROP INDEX IF EXISTS public."IX_workshops_starts_at";
DROP INDEX IF EXISTS public."IX_workshops_instructor_id";
DROP INDEX IF EXISTS public."IX_workshops_code";
DROP INDEX IF EXISTS public."IX_participants_user_id";
DROP INDEX IF EXISTS public."IX_participants_email";
DROP INDEX IF EXISTS public."IX_instructors_user_id";
DROP INDEX IF EXISTS public."IX_instructors_email";
DROP INDEX IF EXISTS public."IX_enrollments_workshop_id_status";
DROP INDEX IF EXISTS public."IX_enrollments_cancelled_by_user_id";
DROP INDEX IF EXISTS public."IX_AspNetUserRoles_RoleId";
DROP INDEX IF EXISTS public."IX_AspNetUserLogins_UserId";
DROP INDEX IF EXISTS public."IX_AspNetUserClaims_UserId";
DROP INDEX IF EXISTS public."IX_AspNetRoleClaims_RoleId";
DROP INDEX IF EXISTS public."EmailIndex";
ALTER TABLE IF EXISTS ONLY public.workshops DROP CONSTRAINT IF EXISTS "PK_workshops";
ALTER TABLE IF EXISTS ONLY public.participants DROP CONSTRAINT IF EXISTS "PK_participants";
ALTER TABLE IF EXISTS ONLY public.instructors DROP CONSTRAINT IF EXISTS "PK_instructors";
ALTER TABLE IF EXISTS ONLY public.enrollments DROP CONSTRAINT IF EXISTS "PK_enrollments";
ALTER TABLE IF EXISTS ONLY public."__EFMigrationsHistory" DROP CONSTRAINT IF EXISTS "PK___EFMigrationsHistory";
ALTER TABLE IF EXISTS ONLY public."AspNetUsers" DROP CONSTRAINT IF EXISTS "PK_AspNetUsers";
ALTER TABLE IF EXISTS ONLY public."AspNetUserTokens" DROP CONSTRAINT IF EXISTS "PK_AspNetUserTokens";
ALTER TABLE IF EXISTS ONLY public."AspNetUserRoles" DROP CONSTRAINT IF EXISTS "PK_AspNetUserRoles";
ALTER TABLE IF EXISTS ONLY public."AspNetUserLogins" DROP CONSTRAINT IF EXISTS "PK_AspNetUserLogins";
ALTER TABLE IF EXISTS ONLY public."AspNetUserClaims" DROP CONSTRAINT IF EXISTS "PK_AspNetUserClaims";
ALTER TABLE IF EXISTS ONLY public."AspNetRoles" DROP CONSTRAINT IF EXISTS "PK_AspNetRoles";
ALTER TABLE IF EXISTS ONLY public."AspNetRoleClaims" DROP CONSTRAINT IF EXISTS "PK_AspNetRoleClaims";
DROP TABLE IF EXISTS public.workshops;
DROP TABLE IF EXISTS public.participants;
DROP TABLE IF EXISTS public.instructors;
DROP TABLE IF EXISTS public.enrollments;
DROP TABLE IF EXISTS public."__EFMigrationsHistory";
DROP TABLE IF EXISTS public."AspNetUsers";
DROP TABLE IF EXISTS public."AspNetUserTokens";
DROP TABLE IF EXISTS public."AspNetUserRoles";
DROP TABLE IF EXISTS public."AspNetUserLogins";
DROP TABLE IF EXISTS public."AspNetUserClaims";
DROP TABLE IF EXISTS public."AspNetRoles";
DROP TABLE IF EXISTS public."AspNetRoleClaims";
SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: AspNetRoleClaims; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AspNetRoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."AspNetRoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetRoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetRoles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AspNetRoles" (
    "Id" text NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


--
-- Name: AspNetUserClaims; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AspNetUserClaims" (
    "Id" integer NOT NULL,
    "UserId" text NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."AspNetUserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AspNetUserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AspNetUserLogins; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" text NOT NULL
);


--
-- Name: AspNetUserRoles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AspNetUserRoles" (
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL
);


--
-- Name: AspNetUserTokens; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AspNetUserTokens" (
    "UserId" text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text
);


--
-- Name: AspNetUsers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AspNetUsers" (
    "Id" text NOT NULL,
    "FullName" text NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- Name: enrollments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.enrollments (
    id uuid NOT NULL,
    workshop_id uuid NOT NULL,
    participant_id uuid NOT NULL,
    status character varying(20) NOT NULL,
    enrolled_at timestamp with time zone NOT NULL,
    cancelled_at timestamp with time zone,
    cancelled_by_user_id text,
    cancelled_by_email character varying(256),
    CONSTRAINT ck_enrollments_cancellation CHECK (((((status)::text = 'Active'::text) AND (cancelled_at IS NULL)) OR (((status)::text = 'Cancelled'::text) AND (cancelled_at IS NOT NULL) AND (cancelled_by_email IS NOT NULL))))
);


--
-- Name: instructors; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.instructors (
    id uuid NOT NULL,
    user_id text NOT NULL,
    full_name character varying(150) NOT NULL,
    email character varying(256) NOT NULL,
    specialty character varying(100) NOT NULL
);


--
-- Name: participants; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.participants (
    id uuid NOT NULL,
    user_id text NOT NULL,
    full_name character varying(150) NOT NULL,
    email character varying(256) NOT NULL,
    registered_at timestamp with time zone NOT NULL
);


--
-- Name: workshops; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.workshops (
    id uuid NOT NULL,
    code character varying(20) NOT NULL,
    title character varying(150) NOT NULL,
    description character varying(1000) NOT NULL,
    instructor_id uuid NOT NULL,
    starts_at timestamp with time zone NOT NULL,
    ends_at timestamp with time zone NOT NULL,
    capacity integer NOT NULL,
    created_at timestamp with time zone NOT NULL,
    CONSTRAINT ck_workshops_capacity CHECK ((capacity > 0)),
    CONSTRAINT ck_workshops_schedule CHECK ((ends_at > starts_at))
);


--
-- Data for Name: AspNetRoleClaims; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."AspNetRoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetRoles; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") FROM stdin;
12cb365f-ff96-45ba-9fd6-2107d4a077c8	Employee	EMPLOYEE	89c31ac9-ad5d-40c0-8968-974fa9f7ba04
db716c58-c62f-482b-8a54-2b556c51b3d4	Participant	PARTICIPANT	23063f6f-9a8d-4919-9f26-6035369c36d4
\.


--
-- Data for Name: AspNetUserClaims; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."AspNetUserClaims" ("Id", "UserId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: AspNetUserLogins; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."AspNetUserLogins" ("LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId") FROM stdin;
\.


--
-- Data for Name: AspNetUserRoles; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."AspNetUserRoles" ("UserId", "RoleId") FROM stdin;
ae3f29eb-df4f-4b3d-8d1d-ecfb279b34e2	12cb365f-ff96-45ba-9fd6-2107d4a077c8
8a2170cd-0fda-46cf-afa7-1bc7ee9e01e0	12cb365f-ff96-45ba-9fd6-2107d4a077c8
fef42b5f-6985-4dd0-9c23-b6b98397e6a5	12cb365f-ff96-45ba-9fd6-2107d4a077c8
cb4704c5-812b-46da-8ec3-f79f86f81301	12cb365f-ff96-45ba-9fd6-2107d4a077c8
a1f6cce0-996c-466d-9ecb-f792d7a8d7cf	12cb365f-ff96-45ba-9fd6-2107d4a077c8
1f1da2f4-71c9-4e07-8334-00e2aa25cd28	12cb365f-ff96-45ba-9fd6-2107d4a077c8
7539f306-68c1-4b0e-bc69-0985b2c4506a	db716c58-c62f-482b-8a54-2b556c51b3d4
5c23df8a-9b3a-4c85-85ab-ad8db21e46b2	db716c58-c62f-482b-8a54-2b556c51b3d4
0a6adbd2-6d0b-487f-bdd9-e85119d898e2	db716c58-c62f-482b-8a54-2b556c51b3d4
bf3540d9-8389-485f-9758-a6a20bd2bb5b	db716c58-c62f-482b-8a54-2b556c51b3d4
c40cda41-86f3-4a4e-9791-954d62c3ad84	db716c58-c62f-482b-8a54-2b556c51b3d4
0fb653bf-35b3-4b31-a12f-77a8084152e4	db716c58-c62f-482b-8a54-2b556c51b3d4
c353e6a1-63e5-413e-848f-0981fb45e9b3	db716c58-c62f-482b-8a54-2b556c51b3d4
8ad69c9a-08c4-4b1f-a6ef-8e80ac2fa0ce	db716c58-c62f-482b-8a54-2b556c51b3d4
0351c91e-7a7f-4f72-b7eb-5865b9b0e742	db716c58-c62f-482b-8a54-2b556c51b3d4
13a8c412-9daa-4d5a-a5d8-897e13bb2b5a	db716c58-c62f-482b-8a54-2b556c51b3d4
695b19d5-61a0-49c1-9992-608e73a01ca3	db716c58-c62f-482b-8a54-2b556c51b3d4
7d50ae2f-5345-423c-95c1-c56f44d93fad	db716c58-c62f-482b-8a54-2b556c51b3d4
65ad883e-e6ce-47cc-bc66-a4e4474222f4	db716c58-c62f-482b-8a54-2b556c51b3d4
69d39e0e-9887-437f-93ea-101dc05d93fc	db716c58-c62f-482b-8a54-2b556c51b3d4
148f9bf9-13d6-4861-9447-8a5bd1e2107c	db716c58-c62f-482b-8a54-2b556c51b3d4
7e5cf212-63ba-4476-822a-a78770ecc82d	db716c58-c62f-482b-8a54-2b556c51b3d4
43be7e12-923d-4ca2-a1c3-d95548bc7e90	db716c58-c62f-482b-8a54-2b556c51b3d4
abf252ad-1bb7-459e-8ff4-25305e37ce8d	db716c58-c62f-482b-8a54-2b556c51b3d4
1e4e4518-ad9f-44bc-a452-236c91d2cd10	db716c58-c62f-482b-8a54-2b556c51b3d4
feb7ebec-58b6-416d-a58c-96fda78a88d5	db716c58-c62f-482b-8a54-2b556c51b3d4
94f24b6e-f523-4225-979a-8c17d6ff01ce	db716c58-c62f-482b-8a54-2b556c51b3d4
be3c210b-3765-4289-9acc-cf1f243a3ef3	db716c58-c62f-482b-8a54-2b556c51b3d4
05a0ad56-9de2-40db-9321-6c454c3177af	db716c58-c62f-482b-8a54-2b556c51b3d4
d78404ea-6379-4b0d-9794-8e99dbb2fbbd	db716c58-c62f-482b-8a54-2b556c51b3d4
de1bc6cb-1af5-46e0-9c7e-e3152cf52fe5	db716c58-c62f-482b-8a54-2b556c51b3d4
2aebd273-86e6-4ecf-bd7b-5e5a5511fd4c	db716c58-c62f-482b-8a54-2b556c51b3d4
b1580cbf-74c3-4823-9fc7-f91058dc4ecf	db716c58-c62f-482b-8a54-2b556c51b3d4
1682041e-a67e-49fb-a078-c9f88b54973f	db716c58-c62f-482b-8a54-2b556c51b3d4
369a66b9-9004-406a-bc12-e167ce874c65	db716c58-c62f-482b-8a54-2b556c51b3d4
c8d4dfff-3359-42dc-8e8b-d7f5c11872cd	db716c58-c62f-482b-8a54-2b556c51b3d4
\.


--
-- Data for Name: AspNetUserTokens; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."AspNetUserTokens" ("UserId", "LoginProvider", "Name", "Value") FROM stdin;
\.


--
-- Data for Name: AspNetUsers; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."AspNetUsers" ("Id", "FullName", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount") FROM stdin;
ae3f29eb-df4f-4b3d-8d1d-ecfb279b34e2	Andrés Felipe Mejía Arango	andres.mejia@correo.com	ANDRES.MEJIA@CORREO.COM	andres.mejia@correo.com	ANDRES.MEJIA@CORREO.COM	t	AQAAAAIAAYagAAAAEIxRki2Zoz1ja1dvXDUIaSag++eITydqqbZh5P28BJxF7L73jeuYaEu2Hee1cdj00w==	ZWWSZMM7NYLF7T4UJVYAYJFLCZH6SULH	f0bfab9e-f8ad-438d-b290-1bb9bc4a9bf5	\N	f	f	\N	t	0
8a2170cd-0fda-46cf-afa7-1bc7ee9e01e0	Rosa Elena Barrios Polo	rosa.barrios@correo.com	ROSA.BARRIOS@CORREO.COM	rosa.barrios@correo.com	ROSA.BARRIOS@CORREO.COM	t	AQAAAAIAAYagAAAAELZlrJAiPRnuR7kRlIfdbrovD4ZNgCar+klc3zwj7yBRJZtn8ajIzzNMP0C7/9Gg4g==	KRZRJSFSBF5TM237OMVVOZZARD6VUULN	dd64ba6d-9aa9-4c0e-a7d2-0fae52caddc9	\N	f	f	\N	t	0
fef42b5f-6985-4dd0-9c23-b6b98397e6a5	Mauricio Jiménez Cárdenas	mauricio.jimenez@correo.com	MAURICIO.JIMENEZ@CORREO.COM	mauricio.jimenez@correo.com	MAURICIO.JIMENEZ@CORREO.COM	t	AQAAAAIAAYagAAAAEOlCj89+X6LRtxYlWFUM+lOw/2UU8G+tgj3SvndZEeVbjYHkXStw6ZI0EmHfKqfi7Q==	EA3JU42TP6HGKWQKXWCAFZFO7KQSC6J3	3920752e-8d0d-450e-a5f7-8a3f7c31f2d8	\N	f	f	\N	t	0
cb4704c5-812b-46da-8ec3-f79f86f81301	Natalia Herrera Castaño	natalia.herrera@correo.com	NATALIA.HERRERA@CORREO.COM	natalia.herrera@correo.com	NATALIA.HERRERA@CORREO.COM	t	AQAAAAIAAYagAAAAELnJKwLsO4XR1G8xRfrcBCSiyCA39iR3cd18HFyhPx2/u2PGvV99Etj8J2rNqjXhDw==	WT6KS5AQLFEKJYAPOO4UDJ2ZHJEQ5UES	8a63db90-c9d9-44c9-b4da-3e9e5803d9a5	\N	f	f	\N	t	0
a1f6cce0-996c-466d-9ecb-f792d7a8d7cf	Hernán Darío Quintero Gil	hernan.quintero@correo.com	HERNAN.QUINTERO@CORREO.COM	hernan.quintero@correo.com	HERNAN.QUINTERO@CORREO.COM	t	AQAAAAIAAYagAAAAEHW/uZ4OarE5Y44KRsy82SvDQRgZagC9dEqWRqVFzf6MGv1KobDVL/1dYyNG+X2+8Q==	IJN6J7KJSFENR5WX4ZYXUKYGCUSRTH26	7abb6811-3cd6-4377-a39b-f069fe497790	\N	f	f	\N	t	0
1f1da2f4-71c9-4e07-8334-00e2aa25cd28	Paola Andrea Vargas Mora	paola.vargas@correo.com	PAOLA.VARGAS@CORREO.COM	paola.vargas@correo.com	PAOLA.VARGAS@CORREO.COM	t	AQAAAAIAAYagAAAAECQqP2nQX1qabvJppLdubUBGNZY2nG23vpsoHnMOHkBlttkAPKAmiKW/2kVUb8kGEw==	IUH7XDFEMZGC7XPHP4HZ2KAJX3HBAYX5	fd7916c6-9c53-4619-981b-ef101e251efd	\N	f	f	\N	t	0
7539f306-68c1-4b0e-bc69-0985b2c4506a	Jorge Andrés Pérez Rojas	jorge.perez@correo.com	JORGE.PEREZ@CORREO.COM	jorge.perez@correo.com	JORGE.PEREZ@CORREO.COM	t	AQAAAAIAAYagAAAAEIhtyjgyAqRhY+RaaekZbX/kcdpvVLElSK2MSJTvXmIBbwrTi4OWRIgAAadeSEWnAA==	NMQWIDUV5YWSHC6JNLF3XZOQHSFG6MHZ	fe8df188-80e3-42fa-80d8-d01c074e18e1	\N	f	f	\N	t	0
5c23df8a-9b3a-4c85-85ab-ad8db21e46b2	Luz Marina Ospina Ríos	luz.ospina@correo.com	LUZ.OSPINA@CORREO.COM	luz.ospina@correo.com	LUZ.OSPINA@CORREO.COM	t	AQAAAAIAAYagAAAAEFBTEWOIB1tpgZ4l6e/1jrVPLUr8tGlP+HRDpzgZB3W0ipPlqUXN1IDRJucMg2rd1Q==	J57EDBD7YZ5PU27CYKCBH5LNJDZVXB4J	2a9087c6-3af0-49da-a40f-3fcd9b6aeff0	\N	f	f	\N	t	0
0a6adbd2-6d0b-487f-bdd9-e85119d898e2	Carlos Eduardo Rincón Díaz	carlos.rincon@correo.com	CARLOS.RINCON@CORREO.COM	carlos.rincon@correo.com	CARLOS.RINCON@CORREO.COM	t	AQAAAAIAAYagAAAAEILPEQJV38hISCctlnCrZ5DWJVuv9lASVs7mYp+NNBulU1zurZc88FJVlu7hnVoHHg==	EQ33WWNXC3HTBL6BHV7R3J4NVMRVBFPA	c8860cc2-caef-40ec-853a-edbfe967de8e	\N	f	f	\N	t	0
bf3540d9-8389-485f-9758-a6a20bd2bb5b	Valentina Gómez Ruiz	valentina.gomez@correo.com	VALENTINA.GOMEZ@CORREO.COM	valentina.gomez@correo.com	VALENTINA.GOMEZ@CORREO.COM	t	AQAAAAIAAYagAAAAEHj25vXmMh4lfKuDSLmqED69Bn1MGAmgMA7lhSHFxegObW9nAKXE+mCavAoqR+Akfw==	4Y4ZP5PR54LBVH6IKEE3BVFRKWFXXTPF	50782ea0-78d7-4ddc-824e-683c5c2b189b	\N	f	f	\N	t	0
c40cda41-86f3-4a4e-9791-954d62c3ad84	Juan Camilo Restrepo Vélez	juan.restrepo@correo.com	JUAN.RESTREPO@CORREO.COM	juan.restrepo@correo.com	JUAN.RESTREPO@CORREO.COM	t	AQAAAAIAAYagAAAAEM2yhw/KkXa9Q91+RuijBttG7Bka9lscQD696MsJbO91Y3XsEUpKrfG9KrUWwOKQQA==	WPBERRHVIEW3VSIYQD377BA2COVVFB6U	526c5aeb-9dfe-4a7f-acd2-edbf59795fe2	\N	f	f	\N	t	0
0fb653bf-35b3-4b31-a12f-77a8084152e4	María Fernanda Torres León	maria.torres@correo.com	MARIA.TORRES@CORREO.COM	maria.torres@correo.com	MARIA.TORRES@CORREO.COM	t	AQAAAAIAAYagAAAAEOPdp3+2GNcLrpdn64ebAq+vU47D0LnqSvAit7UIHSKk1O6LEwjCyW0hRkUAPOYnHQ==	RSG2SK5SIBMNQYWIPVSI3XJZ6XKZCSCJ	c1660dd7-b212-44e3-beb7-5c104a58a00a	\N	f	f	\N	t	0
c353e6a1-63e5-413e-848f-0981fb45e9b3	Santiago Ramírez Henao	santiago.ramirez@correo.com	SANTIAGO.RAMIREZ@CORREO.COM	santiago.ramirez@correo.com	SANTIAGO.RAMIREZ@CORREO.COM	t	AQAAAAIAAYagAAAAEIbTgkOrk3LWAE80EycbNBlWazkz1WR0R6S5bXtOqD8zuM4wdQuhlQWJAh29jPxiwg==	2J273BIE4RK6MWK6BFR7CQX5YA7KK4JB	b6cb3dbb-30aa-4ab3-92e7-dfe0314075d9	\N	f	f	\N	t	0
8ad69c9a-08c4-4b1f-a6ef-8e80ac2fa0ce	Daniela Morales Pineda	daniela.morales@correo.com	DANIELA.MORALES@CORREO.COM	daniela.morales@correo.com	DANIELA.MORALES@CORREO.COM	t	AQAAAAIAAYagAAAAEALZLofZAoQYKwBgxz20QP5a8ODa2+Dqm3TbpisWfMcdZa26rYU/2DOLpkLdtRro0w==	LHW4KA2LJUJ37WPNL55VB42XWCWTEZDC	45b62c42-627d-4978-bbde-f22c8870cd1d	\N	f	f	\N	t	0
0351c91e-7a7f-4f72-b7eb-5865b9b0e742	Sebastián Castro Duque	sebastian.castro@correo.com	SEBASTIAN.CASTRO@CORREO.COM	sebastian.castro@correo.com	SEBASTIAN.CASTRO@CORREO.COM	t	AQAAAAIAAYagAAAAEJy6Fz2p0UFcYHwZQleuHmAL5R57Y5iNtqNjEmSBhwEMWkfHqcEx3X6tFQrlgfaEkQ==	UC2KBTVSC4JRGXHUCM7NMQ3QI4N3DNF4	9c4b2061-0d4a-4bef-86f7-871db2c19f5b	\N	f	f	\N	t	0
13a8c412-9daa-4d5a-a5d8-897e13bb2b5a	Laura Sofía Martínez Suárez	laura.martinez@correo.com	LAURA.MARTINEZ@CORREO.COM	laura.martinez@correo.com	LAURA.MARTINEZ@CORREO.COM	t	AQAAAAIAAYagAAAAEEpROoXcET3S3FRMrtt2DphTNKyVQBRp9FaHOHOOeUzftPfeuN/JfRZhf9n7Rmedow==	DILRO63HGWA7FXWAOYP7GKCWIQJKAHQM	2b86fee5-0006-42b2-a9a0-29f92c01a74a	\N	f	f	\N	t	0
695b19d5-61a0-49c1-9992-608e73a01ca3	Andrés Mauricio Salazar Ortiz	andres.salazar@correo.com	ANDRES.SALAZAR@CORREO.COM	andres.salazar@correo.com	ANDRES.SALAZAR@CORREO.COM	t	AQAAAAIAAYagAAAAEOcJNqf0s+N+Bx8Cd7AFpPWQJ4eCTMlUhlUW50Tb3faLOpnAhM1cE7NK7xTCsisdGw==	HRIAF2GCJP54YUHXK4NRHCXU5U3CAI5T	e9cbed8c-7fc3-4585-818e-df2ed37f7bb1	\N	f	f	\N	t	0
7d50ae2f-5345-423c-95c1-c56f44d93fad	Camila Andrea Cárdenas Ruiz	camila.cardenas@correo.com	CAMILA.CARDENAS@CORREO.COM	camila.cardenas@correo.com	CAMILA.CARDENAS@CORREO.COM	t	AQAAAAIAAYagAAAAEKvpRgBEW60OHilSeamOXD6t8l51RCEOEega6XnkF/KGgnRz7IAt5P2PAUKGWzuHdw==	LHQYV3KH3DVUBHU3665MCDGLMXVYDGGZ	851c7476-0498-4af0-a877-ceccf508b39b	\N	f	f	\N	t	0
65ad883e-e6ce-47cc-bc66-a4e4474222f4	Diego Alejandro Muñoz Peña	diego.munoz@correo.com	DIEGO.MUNOZ@CORREO.COM	diego.munoz@correo.com	DIEGO.MUNOZ@CORREO.COM	t	AQAAAAIAAYagAAAAENXRf0DpsIxxtLVN4dTaD2BwwW4Fv/JBijTCuR2bSckG593AiboR2A/+8cHA8Hm8gg==	J6SDCD3WNBRW4JLO5SNJQL7UM5AGWMC2	2a2a4ff7-dd09-4722-9a53-dc95ab8e6313	\N	f	f	\N	t	0
69d39e0e-9887-437f-93ea-101dc05d93fc	Juliana Ortega Sánchez	juliana.ortega@correo.com	JULIANA.ORTEGA@CORREO.COM	juliana.ortega@correo.com	JULIANA.ORTEGA@CORREO.COM	t	AQAAAAIAAYagAAAAEGzEJe1d0vXiahsTrZ9I8U4nvOMtwG1y+1P2/TSLu7Ksvn8+d/DDedzvai9O+R6sXA==	LOG5YXMJ7VWX54Z6A7VZIOMBDV7L6VMT	177c5e5d-0cf0-4e81-9aa1-0e9c9f160855	\N	f	f	\N	t	0
148f9bf9-13d6-4861-9447-8a5bd1e2107c	Felipe Agudelo Marín	felipe.agudelo@correo.com	FELIPE.AGUDELO@CORREO.COM	felipe.agudelo@correo.com	FELIPE.AGUDELO@CORREO.COM	t	AQAAAAIAAYagAAAAEHyavBaMshMzcwTW8TSDOtBmv0VNrjw9xtDYZAcPXPS+nK/GfEC+HhdwF+uo4aRDYQ==	UJUDRZQLR6WTFGNYFBL4QR5C6ITBYIF2	774971f0-afca-4b8c-8ae8-951eabfb4f4d	\N	f	f	\N	t	0
7e5cf212-63ba-4476-822a-a78770ecc82d	Natalia Montoya Correa	natalia.montoya@correo.com	NATALIA.MONTOYA@CORREO.COM	natalia.montoya@correo.com	NATALIA.MONTOYA@CORREO.COM	t	AQAAAAIAAYagAAAAEB/cEM+6NKQK+IgoXba1g+mKWVJJzICAzjoBqOCEckXsbC+2L4vmTPiSn9y8JYt3VQ==	LYI2GA4DFJE44KR2I4GSGMRYPEI7HMIJ	2e8b56db-d132-41dc-8b7b-3a16a541f2ca	\N	f	f	\N	t	0
43be7e12-923d-4ca2-a1c3-d95548bc7e90	Óscar Iván Guerrero Lozano	oscar.guerrero@correo.com	OSCAR.GUERRERO@CORREO.COM	oscar.guerrero@correo.com	OSCAR.GUERRERO@CORREO.COM	t	AQAAAAIAAYagAAAAEJlX+MkUYqRhP2Q9mDUjsQcmTguJCT7BDkT2XNKSLIFRwYUOxDjt6quqrhXZ5/q1jg==	TSLO3BQ4RQ7NWPANLJKQL36JV3SQHQWF	a0ef0e8f-95ce-48b3-beb2-f0ef27256d5c	\N	f	f	\N	t	0
abf252ad-1bb7-459e-8ff4-25305e37ce8d	Paula Andrea Zapata Ríos	paula.zapata@correo.com	PAULA.ZAPATA@CORREO.COM	paula.zapata@correo.com	PAULA.ZAPATA@CORREO.COM	t	AQAAAAIAAYagAAAAEMVKCETeQkO+1Q6sGpPKYKdZJsTA4hBH978/jUTRr3L4LAJeccYr8uvZRl4bDpvr7g==	CHSI3GE4W3FLOF2VB4ODORUFJJAYOVR2	713390cf-55ff-4380-b924-8110a69efcee	\N	f	f	\N	t	0
1e4e4518-ad9f-44bc-a452-236c91d2cd10	Cristian David Becerra Mejía	cristian.becerra@correo.com	CRISTIAN.BECERRA@CORREO.COM	cristian.becerra@correo.com	CRISTIAN.BECERRA@CORREO.COM	t	AQAAAAIAAYagAAAAEDyOXvWQDyeHdZuPIX0TxNeDfP3AHOj47WHSp2Nr17kjyLFH2uLADfNUKjMzNbv6Xg==	OLH565A6Y6MUJRNMOBSYMFFNFHRCMPG6	c7bb6d53-c2b4-4439-aed8-47cd2644460e	\N	f	f	\N	t	0
feb7ebec-58b6-416d-a58c-96fda78a88d5	Mariana Londoño Álvarez	mariana.londono@correo.com	MARIANA.LONDONO@CORREO.COM	mariana.londono@correo.com	MARIANA.LONDONO@CORREO.COM	t	AQAAAAIAAYagAAAAEKCLQtapIhdZyFM0iJabc7dt/qnaqtvtBs/VVmZFV8l61Vbxm5W0KugY76Zu48EEqg==	73F7JK3RTVXLNHGWDGIQ7PKEPUAXQKVZ	20a7265b-8ac2-43b8-bc5e-937d1cd1e071	\N	f	f	\N	t	0
94f24b6e-f523-4225-979a-8c17d6ff01ce	Javier Alonso Cuéllar Pardo	javier.cuellar@correo.com	JAVIER.CUELLAR@CORREO.COM	javier.cuellar@correo.com	JAVIER.CUELLAR@CORREO.COM	t	AQAAAAIAAYagAAAAECdRtjFIzYjrx4I7nUi9nKT40X/kK95nKdxUvuK9NmTz9kby6E9qV4nDC7cRvbuOXA==	ZUGPDFLSGTLL2DGYBXPQ6VDX24ZQANRA	451d1ba8-1c0a-4127-99de-e3634678e36f	\N	f	f	\N	t	0
be3c210b-3765-4289-9acc-cf1f243a3ef3	Isabella Navarro Fuentes	isabella.navarro@correo.com	ISABELLA.NAVARRO@CORREO.COM	isabella.navarro@correo.com	ISABELLA.NAVARRO@CORREO.COM	t	AQAAAAIAAYagAAAAEDzz350WzNG88JTtZJeRKNITLAMN20cOyYhrSM8Sh5uLOXuxmqFr95GUDXvtqoVNFQ==	ASEV4OCU4DJUJ5XPCOF2I65IOHXQYCPQ	351c0653-01d8-441d-a027-6e1ab93ff773	\N	f	f	\N	t	0
05a0ad56-9de2-40db-9321-6c454c3177af	Kevin Stiven Mosquera Palacios	kevin.mosquera@correo.com	KEVIN.MOSQUERA@CORREO.COM	kevin.mosquera@correo.com	KEVIN.MOSQUERA@CORREO.COM	t	AQAAAAIAAYagAAAAEHfER9MifKSxKhxHDpJgkyax+RW2MxZA9Y8Zffsqday7409/iAyrqgWQzRTkzenEfA==	SHFQQJQERQL36AOD5KXTZZVS3HWKN6OW	b590dac0-d46d-4fc5-af8e-84dafd17b8fe	\N	f	f	\N	t	0
d78404ea-6379-4b0d-9794-8e99dbb2fbbd	Ana María Jaramillo Gaviria	ana.jaramillo@correo.com	ANA.JARAMILLO@CORREO.COM	ana.jaramillo@correo.com	ANA.JARAMILLO@CORREO.COM	t	AQAAAAIAAYagAAAAEPofqxDmiZPqnJSTssR0ojzGMFlfJa0iZfaTHW+xrF0bJviNLzkbG3zdXJdGCFmfUg==	5XDCUD23Y6BZ5EUS7477DCUDJGD66BXC	5ef49635-fe4f-4453-8fee-5c0e79fba7e5	\N	f	f	\N	t	0
de1bc6cb-1af5-46e0-9c7e-e3152cf52fe5	Luis Fernando Arias Cano	luis.arias@correo.com	LUIS.ARIAS@CORREO.COM	luis.arias@correo.com	LUIS.ARIAS@CORREO.COM	t	AQAAAAIAAYagAAAAEND5BiBVcjYr6eKch9PtQhC9MDl662dUbB2k6uae8eB/MA23JYDNLFn6qfEx2Mu1SA==	ANEMEAVF7EACAGLLQMEMOZKW4PALJXIT	c42550d1-e519-42d2-8fff-5ba5fa4e7894	\N	f	f	\N	t	0
2aebd273-86e6-4ecf-bd7b-5e5a5511fd4c	Sara Valencia Bermúdez	sara.valencia@correo.com	SARA.VALENCIA@CORREO.COM	sara.valencia@correo.com	SARA.VALENCIA@CORREO.COM	t	AQAAAAIAAYagAAAAEAJjBrKA90yf0lttKK68IGdoxoR86xV3dSQw4x30QpgQraj9gsqR8WEpOfDeAaPOBw==	QXS4QSP2GT5AVDLIHI2CB4ECHJYX2B5P	c6a36ce3-f689-4a51-82bd-a4dd3347ac5c	\N	f	f	\N	t	0
b1580cbf-74c3-4823-9fc7-f91058dc4ecf	Esteban Rodríguez Molina	esteban.rodriguez@correo.com	ESTEBAN.RODRIGUEZ@CORREO.COM	esteban.rodriguez@correo.com	ESTEBAN.RODRIGUEZ@CORREO.COM	t	AQAAAAIAAYagAAAAEGmr2jiv3ACc80As3iFnIiYJIbh7HW1xWMH9hCy8k/UZrqQ4O+PuhedByENch/2xrA==	SI7EVFHATVVRZRQJ5GT2OMORME3VLE5F	ee2c8a24-1139-447b-8d36-a83bd59d2862	\N	f	f	\N	t	0
1682041e-a67e-49fb-a078-c9f88b54973f	Manuela Escobar Toro	manuela.escobar@correo.com	MANUELA.ESCOBAR@CORREO.COM	manuela.escobar@correo.com	MANUELA.ESCOBAR@CORREO.COM	t	AQAAAAIAAYagAAAAEMM5GWtlbnM3mryS7VYXF05U4jXi1Cn3gNA4JOwAthl2+2DBdxUhNF1RxUfN7U37BA==	4DE3ZE2M7H7GBOPXCQQQLADWZOTS7ICM	a602f514-7ef2-479f-8d69-cac5421676c9	\N	f	f	\N	t	0
369a66b9-9004-406a-bc12-e167ce874c65	Wilmer Alfonso Benítez Cruz	wilmer.benitez@correo.com	WILMER.BENITEZ@CORREO.COM	wilmer.benitez@correo.com	WILMER.BENITEZ@CORREO.COM	t	AQAAAAIAAYagAAAAEFAb4EriO/Ggr/kcZCbYd+d02l42utgzIH8b9AvlUd18xlpYJ1XQHN2sJxqFmC4TBg==	ZMKAWNWOBF7N6ZHBOVLRXTO4LUAFRHNR	25934949-3071-471e-b939-68e2fa07acc7	\N	f	f	\N	t	0
c8d4dfff-3359-42dc-8e8b-d7f5c11872cd	Yuliana Patiño Osorio	yuliana.patino@correo.com	YULIANA.PATINO@CORREO.COM	yuliana.patino@correo.com	YULIANA.PATINO@CORREO.COM	t	AQAAAAIAAYagAAAAENhHw1492C8NIy2GKOQc0caR3ChYhBUyq7FLWGZSu9dtEg9mSz7Ih2Rlv10XiJ9EZw==	3EPNOOESYV7M7WC3OG6BX47GZ2AKE4EB	f2178946-1431-4879-b8b2-d3d81cf3b79b	\N	f	f	\N	t	0
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20261027193941_InitialCreate	10.0.13
\.


--
-- Data for Name: enrollments; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.enrollments (id, workshop_id, participant_id, status, enrolled_at, cancelled_at, cancelled_by_user_id, cancelled_by_email) FROM stdin;
7f7fcff5-fec9-4f31-bd36-a002ffcd35fd	647c7802-7e7d-44ed-88fe-8f46d2200dfa	e77d70f7-5b02-4953-93df-970cbc1dfe87	Active	2026-10-27 14:57:04.498621-05	\N	\N	\N
94fe51e3-99f9-4c05-8549-2b53093239d9	647c7802-7e7d-44ed-88fe-8f46d2200dfa	3fb7bc8c-504e-4295-a727-c136e5f46ebc	Active	2026-10-27 14:57:04.533801-05	\N	\N	\N
44a91d75-2674-4b30-a531-c8832d0532bf	647c7802-7e7d-44ed-88fe-8f46d2200dfa	680d09bc-434c-4f21-9e88-d287a793ffb1	Active	2026-10-27 14:57:04.536364-05	\N	\N	\N
f3e755f5-dc89-42b9-8e17-06eab4e9bb19	647c7802-7e7d-44ed-88fe-8f46d2200dfa	e2ef817f-4490-4dfc-9eed-a8fba1bb88cc	Active	2026-10-27 14:57:04.539012-05	\N	\N	\N
08bd219e-1841-48aa-b17a-2ca049fe483a	647c7802-7e7d-44ed-88fe-8f46d2200dfa	887f1c31-c15a-4423-a6e3-e3925b76fdf1	Active	2026-10-27 14:57:04.541685-05	\N	\N	\N
3a463d16-b9e9-428b-9754-d96c1b96f986	b6334bc0-4f5c-49f9-83a5-2986d14ac2ae	6254e296-b84f-4e17-ba9d-e74bbb0220f3	Active	2026-10-27 14:57:04.544399-05	\N	\N	\N
a7bbdcf3-63ae-4184-ba6c-9cb1cfcb04c3	b6334bc0-4f5c-49f9-83a5-2986d14ac2ae	70574eed-e88a-4da7-9fcd-a3e71816e61f	Active	2026-10-27 14:57:04.546851-05	\N	\N	\N
898edea0-abc7-4af4-a13c-ca31ef3a8bcb	b6334bc0-4f5c-49f9-83a5-2986d14ac2ae	6a802499-83d3-4e8d-8908-7300162bfd57	Active	2026-10-27 14:57:04.549436-05	\N	\N	\N
852b6de2-a6b0-4899-abdf-bafe738e28af	b6334bc0-4f5c-49f9-83a5-2986d14ac2ae	8d0ae483-dc68-481e-abec-efd3963523ef	Active	2026-10-27 14:57:04.552081-05	\N	\N	\N
8eef6040-bccd-4ce3-83c0-e61d80b779a4	b6334bc0-4f5c-49f9-83a5-2986d14ac2ae	1ab019a6-8f02-467a-ac57-15d05c1d4b81	Active	2026-10-27 14:57:04.554728-05	\N	\N	\N
44e41a8e-9b3e-4b58-aa6b-20ff44224915	0b7d1f5d-6a55-4779-97f0-3ae30fd844af	7f13c2bb-04ea-464b-8788-7ff65af671e5	Active	2026-10-27 14:57:04.556911-05	\N	\N	\N
243bac33-83bc-41d7-972c-92c39343ef84	0b7d1f5d-6a55-4779-97f0-3ae30fd844af	18e88c50-25aa-432e-9508-38d28b62c7b8	Active	2026-10-27 14:57:04.559543-05	\N	\N	\N
8aee2908-0e9c-48f7-8a1f-7797feed0af9	0b7d1f5d-6a55-4779-97f0-3ae30fd844af	8d0ae483-dc68-481e-abec-efd3963523ef	Active	2026-10-27 14:57:04.561655-05	\N	\N	\N
7f7c8b44-19e8-4d5a-84b9-bf3a1af540c3	0b7d1f5d-6a55-4779-97f0-3ae30fd844af	aab2ec0a-a673-413b-a87e-f3f356773332	Active	2026-10-27 14:57:04.564057-05	\N	\N	\N
c3f925a7-ee38-423a-b41d-11abb33a69ae	0b7d1f5d-6a55-4779-97f0-3ae30fd844af	3cb4c591-386a-4aca-94fe-deebc4f33031	Active	2026-10-27 14:57:04.566007-05	\N	\N	\N
037a1a09-30ca-4b11-9fc5-e6c687cff016	0b7d1f5d-6a55-4779-97f0-3ae30fd844af	d09fd373-e68e-42c3-a841-7392818e331e	Active	2026-10-27 14:57:04.568305-05	\N	\N	\N
28d1aca7-77b8-40c6-ae39-124bb7a6dc44	f2b19590-c98e-4e2f-8601-08ff87fc1cff	5b278b06-2576-4722-907b-e8f793ee15d5	Active	2026-10-27 14:57:04.570409-05	\N	\N	\N
2ac3698e-de93-4f2a-9a2d-2a7d277beaf7	f2b19590-c98e-4e2f-8601-08ff87fc1cff	8d0ae483-dc68-481e-abec-efd3963523ef	Active	2026-10-27 14:57:04.572376-05	\N	\N	\N
ce9d599b-f7a3-485c-8db0-27c41bd3d596	f2b19590-c98e-4e2f-8601-08ff87fc1cff	887f1c31-c15a-4423-a6e3-e3925b76fdf1	Active	2026-10-27 14:57:04.57437-05	\N	\N	\N
d0ff998d-07cd-47d1-9d19-e2de3dfea035	f2b19590-c98e-4e2f-8601-08ff87fc1cff	2f489434-a476-479b-b352-e5fc1c868bba	Active	2026-10-27 14:57:04.577341-05	\N	\N	\N
32889bee-3cb0-4cc6-a9b4-fefbdd36daeb	65b90596-fd56-4731-9834-9944d5c25382	875559f0-868d-4c9d-a79e-76b3c4f20720	Active	2026-10-27 14:57:04.580041-05	\N	\N	\N
0bc59f2a-ee56-4103-8fc5-fd40277dc485	65b90596-fd56-4731-9834-9944d5c25382	18e88c50-25aa-432e-9508-38d28b62c7b8	Active	2026-10-27 14:57:04.582581-05	\N	\N	\N
94e347f6-dc05-486c-b1c0-7b92755391d6	65b90596-fd56-4731-9834-9944d5c25382	aab2ec0a-a673-413b-a87e-f3f356773332	Active	2026-10-27 14:57:04.585198-05	\N	\N	\N
d89a4773-942d-422f-b020-d05308c193b0	65b90596-fd56-4731-9834-9944d5c25382	422fc5d2-fe57-42b6-bcd3-002621688895	Active	2026-10-27 14:57:04.587732-05	\N	\N	\N
94b41dd9-2cfd-408b-8ffd-79fdc14e3029	65b90596-fd56-4731-9834-9944d5c25382	73f1b9e8-a5b6-462e-a20f-cad8988a6a9f	Active	2026-10-27 14:57:04.590075-05	\N	\N	\N
9c9cbe44-cb5c-471c-a537-6e6e0b927d2c	0ae9f6f2-75d4-4c53-95ec-a7d25da6b700	5b278b06-2576-4722-907b-e8f793ee15d5	Active	2026-10-27 14:57:04.59246-05	\N	\N	\N
2a71a741-1478-41b1-96b6-fc14c5340be6	0ae9f6f2-75d4-4c53-95ec-a7d25da6b700	422fc5d2-fe57-42b6-bcd3-002621688895	Active	2026-10-27 14:57:04.594496-05	\N	\N	\N
de3d9e08-b4c3-441d-8fc1-0002c3df1b82	0ae9f6f2-75d4-4c53-95ec-a7d25da6b700	3e3c0fc7-45f5-4a12-813d-9c08551531da	Active	2026-10-27 14:57:04.596523-05	\N	\N	\N
88d22b72-d96c-403a-aeb3-908314b7971c	c30ab775-4f77-4e10-a6b7-5f3e2a1b29b0	2c89a261-bc75-468e-9c14-18d3cba4d2bb	Active	2026-10-27 14:57:04.59847-05	\N	\N	\N
73c615ef-8df9-4004-b9ef-77a2e39a859b	c30ab775-4f77-4e10-a6b7-5f3e2a1b29b0	e77d70f7-5b02-4953-93df-970cbc1dfe87	Active	2026-10-27 14:57:04.600638-05	\N	\N	\N
2710f87c-e3f4-422d-acac-a63df04caa49	c30ab775-4f77-4e10-a6b7-5f3e2a1b29b0	92e8f2fd-caee-4b45-887d-0389ed3d8b3a	Active	2026-10-27 14:57:04.602763-05	\N	\N	\N
85e9305c-77d2-4f69-add0-4fff09ad15ef	8e9460de-3d1b-483a-9253-78b2a29b2d65	6a802499-83d3-4e8d-8908-7300162bfd57	Active	2026-10-27 14:57:04.604676-05	\N	\N	\N
c424239f-30e2-4455-a734-47e5a6024d1a	8e9460de-3d1b-483a-9253-78b2a29b2d65	73f1b9e8-a5b6-462e-a20f-cad8988a6a9f	Active	2026-10-27 14:57:04.606616-05	\N	\N	\N
fdf0af84-2140-413d-a97f-5629ab55f8e2	8e9460de-3d1b-483a-9253-78b2a29b2d65	2f489434-a476-479b-b352-e5fc1c868bba	Active	2026-10-27 14:57:04.608744-05	\N	\N	\N
b668b578-fc8c-4c45-ab22-2e1618552ce5	8e9460de-3d1b-483a-9253-78b2a29b2d65	d09fd373-e68e-42c3-a841-7392818e331e	Active	2026-10-27 14:57:04.610796-05	\N	\N	\N
b2cebdd9-a38f-47d5-a0d5-6f86e52dbd35	e56555ef-f8eb-4d03-b2d3-94b769ed7782	e77d70f7-5b02-4953-93df-970cbc1dfe87	Active	2026-10-27 14:57:04.612847-05	\N	\N	\N
584cedc5-a67f-406a-980d-3487158c2281	e56555ef-f8eb-4d03-b2d3-94b769ed7782	6254e296-b84f-4e17-ba9d-e74bbb0220f3	Active	2026-10-27 14:57:04.614837-05	\N	\N	\N
54da48a0-42e7-4b26-a5c6-1bb87c4b9841	e56555ef-f8eb-4d03-b2d3-94b769ed7782	18e88c50-25aa-432e-9508-38d28b62c7b8	Active	2026-10-27 14:57:04.617093-05	\N	\N	\N
7a6f2bb1-29d3-45f3-b4ff-dd855b404857	e56555ef-f8eb-4d03-b2d3-94b769ed7782	70574eed-e88a-4da7-9fcd-a3e71816e61f	Active	2026-10-27 14:57:04.619229-05	\N	\N	\N
1ae302f5-52d4-4a32-9024-71b153f8bab3	e56555ef-f8eb-4d03-b2d3-94b769ed7782	92e8f2fd-caee-4b45-887d-0389ed3d8b3a	Active	2026-10-27 14:57:04.621131-05	\N	\N	\N
964118fb-a40b-45cf-b00c-78c67342b040	e56555ef-f8eb-4d03-b2d3-94b769ed7782	3cb4c591-386a-4aca-94fe-deebc4f33031	Active	2026-10-27 14:57:04.623008-05	\N	\N	\N
eecc489f-1348-4731-9fb3-9f2621b07dba	4131792e-41cf-4b9e-9b2d-4cf2448e4af8	4d0a5c3d-3965-4936-ae13-134686c62ffa	Active	2026-10-27 14:57:04.625271-05	\N	\N	\N
bcdb3e3f-c462-486c-a980-8ca567b30d38	4131792e-41cf-4b9e-9b2d-4cf2448e4af8	7f13c2bb-04ea-464b-8788-7ff65af671e5	Active	2026-10-27 14:57:04.627469-05	\N	\N	\N
deea732b-948e-465b-a92a-1040d54a9f52	4131792e-41cf-4b9e-9b2d-4cf2448e4af8	d64650f0-3030-4234-95fa-6e2d9c7be92e	Active	2026-10-27 14:57:04.629634-05	\N	\N	\N
ea9c580e-0c06-4b41-8cb9-b55989ab71cd	4131792e-41cf-4b9e-9b2d-4cf2448e4af8	3fb7bc8c-504e-4295-a727-c136e5f46ebc	Active	2026-10-27 14:57:04.631833-05	\N	\N	\N
31724335-4b89-44bd-8275-688c265fdcda	4131792e-41cf-4b9e-9b2d-4cf2448e4af8	73f1b9e8-a5b6-462e-a20f-cad8988a6a9f	Active	2026-10-27 14:57:04.634073-05	\N	\N	\N
b3cc246d-1bad-4175-a3d9-cbdac36654d0	4131792e-41cf-4b9e-9b2d-4cf2448e4af8	3cb4c591-386a-4aca-94fe-deebc4f33031	Active	2026-10-27 14:57:04.63676-05	\N	\N	\N
acf2d089-dced-404b-b4dd-bfe7685cfde8	4131792e-41cf-4b9e-9b2d-4cf2448e4af8	151a47a6-43d5-4d07-9085-716146dbf84f	Active	2026-10-27 14:57:04.640194-05	\N	\N	\N
d7ae50a5-0959-4b38-9309-16fb29d192c3	441160b1-e3a3-4ebe-8809-e2a4bf078100	4d0a5c3d-3965-4936-ae13-134686c62ffa	Active	2026-10-27 14:57:04.644129-05	\N	\N	\N
299cc9d4-6571-4f61-9c87-b2a3d01e5c14	441160b1-e3a3-4ebe-8809-e2a4bf078100	3fb7bc8c-504e-4295-a727-c136e5f46ebc	Active	2026-10-27 14:57:04.6466-05	\N	\N	\N
5d5087a2-2dcb-44fc-b916-59ce980186d8	441160b1-e3a3-4ebe-8809-e2a4bf078100	aab2ec0a-a673-413b-a87e-f3f356773332	Active	2026-10-27 14:57:04.648706-05	\N	\N	\N
ccbb097d-11b0-4c72-b77f-00209ab7e479	441160b1-e3a3-4ebe-8809-e2a4bf078100	422fc5d2-fe57-42b6-bcd3-002621688895	Active	2026-10-27 14:57:04.650921-05	\N	\N	\N
707dafb7-9408-485e-aae1-53646bff9801	887815b7-0de3-4b2d-953c-e014d3ea2eff	4d0a5c3d-3965-4936-ae13-134686c62ffa	Active	2026-10-27 14:57:04.653039-05	\N	\N	\N
03633715-9ef8-4ec0-9614-3c1899e13aa3	887815b7-0de3-4b2d-953c-e014d3ea2eff	bfbe80be-26f3-4f6b-b699-a9bfe6d3cf2a	Active	2026-10-27 14:57:04.65529-05	\N	\N	\N
bfe6611f-be54-4fa3-99d3-50172813bb80	887815b7-0de3-4b2d-953c-e014d3ea2eff	1ab019a6-8f02-467a-ac57-15d05c1d4b81	Active	2026-10-27 14:57:04.657425-05	\N	\N	\N
96a9bade-c916-476f-b79f-01bd99b6429a	887815b7-0de3-4b2d-953c-e014d3ea2eff	aab2ec0a-a673-413b-a87e-f3f356773332	Active	2026-10-27 14:57:04.660042-05	\N	\N	\N
fc07187a-15f8-406d-b3e6-5909ef80b6f1	887815b7-0de3-4b2d-953c-e014d3ea2eff	92e8f2fd-caee-4b45-887d-0389ed3d8b3a	Active	2026-10-27 14:57:04.662239-05	\N	\N	\N
39c02d8a-d57a-4b22-81fd-f6bded356d17	887815b7-0de3-4b2d-953c-e014d3ea2eff	887f1c31-c15a-4423-a6e3-e3925b76fdf1	Active	2026-10-27 14:57:04.664163-05	\N	\N	\N
1c3be33e-c5ab-4688-9b63-9f0fb234f99b	90ff21f6-bde6-41f9-88b5-366ccde03864	2f489434-a476-479b-b352-e5fc1c868bba	Active	2026-10-27 14:57:04.666157-05	\N	\N	\N
e5716a15-7eef-4941-a876-1895b0af43b9	90ff21f6-bde6-41f9-88b5-366ccde03864	151a47a6-43d5-4d07-9085-716146dbf84f	Active	2026-10-27 14:57:04.668382-05	\N	\N	\N
ebdf343a-fa2c-4d36-b8ff-03ac68dbb207	13fa24df-46ea-4f7c-b10a-07de44fc7e4d	875559f0-868d-4c9d-a79e-76b3c4f20720	Active	2026-10-27 14:57:04.670345-05	\N	\N	\N
3164c19c-87f1-4043-ab14-113f5dc558b1	13fa24df-46ea-4f7c-b10a-07de44fc7e4d	422fc5d2-fe57-42b6-bcd3-002621688895	Active	2026-10-27 14:57:04.672358-05	\N	\N	\N
82fa297b-ea29-4e34-af92-4c59948fb964	13fa24df-46ea-4f7c-b10a-07de44fc7e4d	887f1c31-c15a-4423-a6e3-e3925b76fdf1	Active	2026-10-27 14:57:04.674316-05	\N	\N	\N
532a68eb-fe1b-49d7-958f-ab02863d3b48	43d68f4e-1b09-44d0-94c8-d08d2e095d33	e77d70f7-5b02-4953-93df-970cbc1dfe87	Active	2026-10-27 14:57:04.68323-05	\N	\N	\N
30b11928-e4ed-4eca-acaa-df962d162da5	43d68f4e-1b09-44d0-94c8-d08d2e095d33	8d0ae483-dc68-481e-abec-efd3963523ef	Active	2026-10-27 14:57:04.685397-05	\N	\N	\N
b1cb7911-3e46-4c53-85f8-f91722fca6dd	43d68f4e-1b09-44d0-94c8-d08d2e095d33	1ab019a6-8f02-467a-ac57-15d05c1d4b81	Active	2026-10-27 14:57:04.687325-05	\N	\N	\N
9fbcb4d2-3772-4dd5-ab65-8e38c761d3aa	43d68f4e-1b09-44d0-94c8-d08d2e095d33	680d09bc-434c-4f21-9e88-d287a793ffb1	Active	2026-10-27 14:57:04.689561-05	\N	\N	\N
ae4b63f4-0094-4125-9436-5b20bac3e41a	43d68f4e-1b09-44d0-94c8-d08d2e095d33	31d94211-4a15-4935-b17b-779bcfc80c2b	Active	2026-10-27 14:57:04.692328-05	\N	\N	\N
893021d4-483b-4af6-ad6f-8e34f2c04aaf	43d68f4e-1b09-44d0-94c8-d08d2e095d33	e2ef817f-4490-4dfc-9eed-a8fba1bb88cc	Active	2026-10-27 14:57:04.69514-05	\N	\N	\N
255b0677-b9c7-4e1e-a871-c64884870281	43d68f4e-1b09-44d0-94c8-d08d2e095d33	2f489434-a476-479b-b352-e5fc1c868bba	Active	2026-10-27 14:57:04.697525-05	\N	\N	\N
\.


--
-- Data for Name: instructors; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.instructors (id, user_id, full_name, email, specialty) FROM stdin;
63887fad-90cd-45db-b1d0-cd8979a03033	ae3f29eb-df4f-4b3d-8d1d-ecfb279b34e2	Andrés Felipe Mejía Arango	andres.mejia@correo.com	Fotografía
2a9acee1-a784-4250-a4de-274786e98985	8a2170cd-0fda-46cf-afa7-1bc7ee9e01e0	Rosa Elena Barrios Polo	rosa.barrios@correo.com	Cocina
c3e7d75f-e14d-4d12-85e1-3397db576ffa	fef42b5f-6985-4dd0-9c23-b6b98397e6a5	Mauricio Jiménez Cárdenas	mauricio.jimenez@correo.com	Oratoria
37e2d462-e5d3-48ed-91fa-37f187ce00a8	cb4704c5-812b-46da-8ec3-f79f86f81301	Natalia Herrera Castaño	natalia.herrera@correo.com	Diseño gráfico
de9f02da-4615-49bc-b754-3d5333494a57	a1f6cce0-996c-466d-9ecb-f792d7a8d7cf	Hernán Darío Quintero Gil	hernan.quintero@correo.com	Música
09389808-3d1f-4a39-93a9-0ff2797b6aae	1f1da2f4-71c9-4e07-8334-00e2aa25cd28	Paola Andrea Vargas Mora	paola.vargas@correo.com	Marketing digital
\.


--
-- Data for Name: participants; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.participants (id, user_id, full_name, email, registered_at) FROM stdin;
3fb7bc8c-504e-4295-a727-c136e5f46ebc	7539f306-68c1-4b0e-bc69-0985b2c4506a	Jorge Andrés Pérez Rojas	jorge.perez@correo.com	2026-10-27 14:56:56.396979-05
92e8f2fd-caee-4b45-887d-0389ed3d8b3a	5c23df8a-9b3a-4c85-85ab-ad8db21e46b2	Luz Marina Ospina Ríos	luz.ospina@correo.com	2026-10-27 14:56:56.473886-05
875559f0-868d-4c9d-a79e-76b3c4f20720	0a6adbd2-6d0b-487f-bdd9-e85119d898e2	Carlos Eduardo Rincón Díaz	carlos.rincon@correo.com	2026-10-27 14:56:56.534442-05
3e3c0fc7-45f5-4a12-813d-9c08551531da	bf3540d9-8389-485f-9758-a6a20bd2bb5b	Valentina Gómez Ruiz	valentina.gomez@correo.com	2026-10-27 14:56:56.59547-05
6a802499-83d3-4e8d-8908-7300162bfd57	c40cda41-86f3-4a4e-9791-954d62c3ad84	Juan Camilo Restrepo Vélez	juan.restrepo@correo.com	2026-10-27 14:56:56.658394-05
e2ef817f-4490-4dfc-9eed-a8fba1bb88cc	0fb653bf-35b3-4b31-a12f-77a8084152e4	María Fernanda Torres León	maria.torres@correo.com	2026-10-27 14:56:56.723265-05
3cb4c591-386a-4aca-94fe-deebc4f33031	c353e6a1-63e5-413e-848f-0981fb45e9b3	Santiago Ramírez Henao	santiago.ramirez@correo.com	2026-10-27 14:56:56.786879-05
e77d70f7-5b02-4953-93df-970cbc1dfe87	8ad69c9a-08c4-4b1f-a6ef-8e80ac2fa0ce	Daniela Morales Pineda	daniela.morales@correo.com	2026-10-27 14:56:56.851768-05
151a47a6-43d5-4d07-9085-716146dbf84f	0351c91e-7a7f-4f72-b7eb-5865b9b0e742	Sebastián Castro Duque	sebastian.castro@correo.com	2026-10-27 14:56:56.918859-05
680d09bc-434c-4f21-9e88-d287a793ffb1	13a8c412-9daa-4d5a-a5d8-897e13bb2b5a	Laura Sofía Martínez Suárez	laura.martinez@correo.com	2026-10-27 14:56:56.980045-05
7f13c2bb-04ea-464b-8788-7ff65af671e5	695b19d5-61a0-49c1-9992-608e73a01ca3	Andrés Mauricio Salazar Ortiz	andres.salazar@correo.com	2026-10-27 14:56:57.050816-05
2c89a261-bc75-468e-9c14-18d3cba4d2bb	7d50ae2f-5345-423c-95c1-c56f44d93fad	Camila Andrea Cárdenas Ruiz	camila.cardenas@correo.com	2026-10-27 14:56:57.112806-05
bfbe80be-26f3-4f6b-b699-a9bfe6d3cf2a	65ad883e-e6ce-47cc-bc66-a4e4474222f4	Diego Alejandro Muñoz Peña	diego.munoz@correo.com	2026-10-27 14:56:57.176212-05
8d0ae483-dc68-481e-abec-efd3963523ef	69d39e0e-9887-437f-93ea-101dc05d93fc	Juliana Ortega Sánchez	juliana.ortega@correo.com	2026-10-27 14:56:57.237521-05
6254e296-b84f-4e17-ba9d-e74bbb0220f3	148f9bf9-13d6-4861-9447-8a5bd1e2107c	Felipe Agudelo Marín	felipe.agudelo@correo.com	2026-10-27 14:56:57.308271-05
422fc5d2-fe57-42b6-bcd3-002621688895	7e5cf212-63ba-4476-822a-a78770ecc82d	Natalia Montoya Correa	natalia.montoya@correo.com	2026-10-27 14:56:57.370794-05
887f1c31-c15a-4423-a6e3-e3925b76fdf1	43be7e12-923d-4ca2-a1c3-d95548bc7e90	Óscar Iván Guerrero Lozano	oscar.guerrero@correo.com	2026-10-27 14:56:57.43251-05
73f1b9e8-a5b6-462e-a20f-cad8988a6a9f	abf252ad-1bb7-459e-8ff4-25305e37ce8d	Paula Andrea Zapata Ríos	paula.zapata@correo.com	2026-10-27 14:56:57.494015-05
d64650f0-3030-4234-95fa-6e2d9c7be92e	1e4e4518-ad9f-44bc-a452-236c91d2cd10	Cristian David Becerra Mejía	cristian.becerra@correo.com	2026-10-27 14:56:57.555621-05
b0815e6c-5250-48d2-a3ab-eae3b45e000b	feb7ebec-58b6-416d-a58c-96fda78a88d5	Mariana Londoño Álvarez	mariana.londono@correo.com	2026-10-27 14:56:57.62019-05
70574eed-e88a-4da7-9fcd-a3e71816e61f	94f24b6e-f523-4225-979a-8c17d6ff01ce	Javier Alonso Cuéllar Pardo	javier.cuellar@correo.com	2026-10-27 14:56:57.683279-05
18e88c50-25aa-432e-9508-38d28b62c7b8	be3c210b-3765-4289-9acc-cf1f243a3ef3	Isabella Navarro Fuentes	isabella.navarro@correo.com	2026-10-27 14:56:57.746642-05
1ab019a6-8f02-467a-ac57-15d05c1d4b81	05a0ad56-9de2-40db-9321-6c454c3177af	Kevin Stiven Mosquera Palacios	kevin.mosquera@correo.com	2026-10-27 14:56:57.811071-05
4d0a5c3d-3965-4936-ae13-134686c62ffa	d78404ea-6379-4b0d-9794-8e99dbb2fbbd	Ana María Jaramillo Gaviria	ana.jaramillo@correo.com	2026-10-27 14:56:57.87307-05
aab2ec0a-a673-413b-a87e-f3f356773332	de1bc6cb-1af5-46e0-9c7e-e3152cf52fe5	Luis Fernando Arias Cano	luis.arias@correo.com	2026-10-27 14:56:57.936276-05
2f489434-a476-479b-b352-e5fc1c868bba	2aebd273-86e6-4ecf-bd7b-5e5a5511fd4c	Sara Valencia Bermúdez	sara.valencia@correo.com	2026-10-27 14:56:57.998638-05
5b278b06-2576-4722-907b-e8f793ee15d5	b1580cbf-74c3-4823-9fc7-f91058dc4ecf	Esteban Rodríguez Molina	esteban.rodriguez@correo.com	2026-10-27 14:56:58.063223-05
31d94211-4a15-4935-b17b-779bcfc80c2b	1682041e-a67e-49fb-a078-c9f88b54973f	Manuela Escobar Toro	manuela.escobar@correo.com	2026-10-27 14:56:58.126895-05
d09fd373-e68e-42c3-a841-7392818e331e	369a66b9-9004-406a-bc12-e167ce874c65	Wilmer Alfonso Benítez Cruz	wilmer.benitez@correo.com	2026-10-27 14:56:58.189544-05
c0e6586d-3e89-48c5-a194-7306077cdca3	c8d4dfff-3359-42dc-8e8b-d7f5c11872cd	Yuliana Patiño Osorio	yuliana.patino@correo.com	2026-10-27 14:56:58.252399-05
\.


--
-- Data for Name: workshops; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.workshops (id, code, title, description, instructor_id, starts_at, ends_at, capacity, created_at) FROM stdin;
0ae9f6f2-75d4-4c53-95ec-a7d25da6b700	FOT-001	Fotografía con celular	Composición, luz y edición básica desde el celular	63887fad-90cd-45db-b1d0-cd8979a03033	2026-12-05 08:00:00-05	2026-12-05 12:00:00-05	15	2026-10-27 14:56:58.266062-05
0b7d1f5d-6a55-4779-97f0-3ae30fd844af	COC-003	Cocina saludable	Platos balanceados para la semana	2a9acee1-a784-4250-a4de-274786e98985	2027-02-13 09:00:00-05	2027-02-13 12:00:00-05	14	2026-10-27 14:56:58.289497-05
13fa24df-46ea-4f7c-b10a-07de44fc7e4d	ORA-001	Oratoria para principiantes	Técnicas para hablar en público con seguridad	c3e7d75f-e14d-4d12-85e1-3397db576ffa	2026-12-12 09:00:00-05	2026-12-12 12:00:00-05	20	2026-10-27 14:56:58.290021-05
4131792e-41cf-4b9e-9b2d-4cf2448e4af8	MKT-002	Publicidad en Meta e Instagram	Configuración y medición de campañas	09389808-3d1f-4a39-93a9-0ff2797b6aae	2027-01-16 14:00:00-05	2027-01-16 18:00:00-05	16	2026-10-27 14:56:58.293637-05
43d68f4e-1b09-44d0-94c8-d08d2e095d33	ORA-002	Presentaciones efectivas	Cómo estructurar y presentar una idea en 10 minutos	c3e7d75f-e14d-4d12-85e1-3397db576ffa	2027-01-23 09:00:00-05	2027-01-23 12:00:00-05	18	2026-10-27 14:56:58.290531-05
441160b1-e3a3-4ebe-8809-e2a4bf078100	MKT-003	Emprendimiento digital	Validar y lanzar una idea de negocio en línea	09389808-3d1f-4a39-93a9-0ff2797b6aae	2027-02-27 09:00:00-05	2027-02-27 12:00:00-05	20	2026-10-27 14:56:58.29411-05
647c7802-7e7d-44ed-88fe-8f46d2200dfa	COC-001	Cocina costeña	Arroz con coco, patacones y mojarra frita	2a9acee1-a784-4250-a4de-274786e98985	2026-12-05 14:00:00-05	2026-12-05 18:00:00-05	12	2026-10-27 14:56:58.288394-05
65b90596-fd56-4731-9834-9944d5c25382	DIS-002	Fundamentos de color y tipografía	Principios visuales para piezas profesionales	37e2d462-e5d3-48ed-91fa-37f187ce00a8	2027-01-30 14:00:00-05	2027-01-30 17:00:00-05	15	2026-10-27 14:56:58.29157-05
887815b7-0de3-4b2d-953c-e014d3ea2eff	MUS-001	Guitarra desde cero	Acordes básicos y primeras canciones	de9f02da-4615-49bc-b754-3d5333494a57	2026-12-12 14:00:00-05	2026-12-12 17:00:00-05	10	2026-10-27 14:56:58.292197-05
8e9460de-3d1b-483a-9253-78b2a29b2d65	FOT-003	Edición en Lightroom	Flujo de revelado y ajustes de color	63887fad-90cd-45db-b1d0-cd8979a03033	2027-02-06 14:00:00-05	2027-02-06 18:00:00-05	12	2026-10-27 14:56:58.287833-05
90ff21f6-bde6-41f9-88b5-366ccde03864	MUS-002	Producción musical en casa	Grabación y mezcla con herramientas gratuitas	de9f02da-4615-49bc-b754-3d5333494a57	2027-02-20 14:00:00-05	2027-02-20 18:00:00-05	12	2026-10-27 14:56:58.292709-05
b6334bc0-4f5c-49f9-83a5-2986d14ac2ae	COC-002	Repostería básica	Tortas, galletas y postres de nevera	2a9acee1-a784-4250-a4de-274786e98985	2027-01-09 09:00:00-05	2027-01-09 13:00:00-05	10	2026-10-27 14:56:58.288946-05
c30ab775-4f77-4e10-a6b7-5f3e2a1b29b0	FOT-002	Retrato en exteriores	Manejo de luz natural para retratos al aire libre	63887fad-90cd-45db-b1d0-cd8979a03033	2027-01-16 08:00:00-05	2027-01-16 12:00:00-05	10	2026-10-27 14:56:58.287201-05
e56555ef-f8eb-4d03-b2d3-94b769ed7782	MKT-001	Marketing en redes sociales	Estrategia de contenido para pequeños negocios	09389808-3d1f-4a39-93a9-0ff2797b6aae	2026-12-19 08:00:00-05	2026-12-19 12:00:00-05	20	2026-10-27 14:56:58.293166-05
f2b19590-c98e-4e2f-8601-08ff87fc1cff	DIS-001	Diseño en Canva	Piezas gráficas para redes sociales	37e2d462-e5d3-48ed-91fa-37f187ce00a8	2026-12-19 14:00:00-05	2026-12-19 17:00:00-05	18	2026-10-27 14:56:58.291036-05
\.


--
-- Name: AspNetRoleClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."AspNetRoleClaims_Id_seq"', 1, false);


--
-- Name: AspNetUserClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."AspNetUserClaims_Id_seq"', 1, false);


--
-- Name: AspNetRoleClaims PK_AspNetRoleClaims; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetRoles PK_AspNetRoles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetRoles"
    ADD CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id");


--
-- Name: AspNetUserClaims PK_AspNetUserClaims; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id");


--
-- Name: AspNetUserLogins PK_AspNetUserLogins; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: AspNetUserRoles PK_AspNetUserRoles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: AspNetUserTokens PK_AspNetUserTokens; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: AspNetUsers PK_AspNetUsers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUsers"
    ADD CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: enrollments PK_enrollments; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.enrollments
    ADD CONSTRAINT "PK_enrollments" PRIMARY KEY (id);


--
-- Name: instructors PK_instructors; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.instructors
    ADD CONSTRAINT "PK_instructors" PRIMARY KEY (id);


--
-- Name: participants PK_participants; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.participants
    ADD CONSTRAINT "PK_participants" PRIMARY KEY (id);


--
-- Name: workshops PK_workshops; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.workshops
    ADD CONSTRAINT "PK_workshops" PRIMARY KEY (id);


--
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "EmailIndex" ON public."AspNetUsers" USING btree ("NormalizedEmail");


--
-- Name: IX_AspNetRoleClaims_RoleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON public."AspNetRoleClaims" USING btree ("RoleId");


--
-- Name: IX_AspNetUserClaims_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AspNetUserClaims_UserId" ON public."AspNetUserClaims" USING btree ("UserId");


--
-- Name: IX_AspNetUserLogins_UserId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AspNetUserLogins_UserId" ON public."AspNetUserLogins" USING btree ("UserId");


--
-- Name: IX_AspNetUserRoles_RoleId; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON public."AspNetUserRoles" USING btree ("RoleId");


--
-- Name: IX_enrollments_cancelled_by_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_enrollments_cancelled_by_user_id" ON public.enrollments USING btree (cancelled_by_user_id);


--
-- Name: IX_enrollments_workshop_id_status; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_enrollments_workshop_id_status" ON public.enrollments USING btree (workshop_id, status);


--
-- Name: IX_instructors_email; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_instructors_email" ON public.instructors USING btree (email);


--
-- Name: IX_instructors_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_instructors_user_id" ON public.instructors USING btree (user_id);


--
-- Name: IX_participants_email; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_participants_email" ON public.participants USING btree (email);


--
-- Name: IX_participants_user_id; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_participants_user_id" ON public.participants USING btree (user_id);


--
-- Name: IX_workshops_code; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_workshops_code" ON public.workshops USING btree (code);


--
-- Name: IX_workshops_instructor_id; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_workshops_instructor_id" ON public.workshops USING btree (instructor_id);


--
-- Name: IX_workshops_starts_at; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_workshops_starts_at" ON public.workshops USING btree (starts_at);


--
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."AspNetRoles" USING btree ("NormalizedName");


--
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."AspNetUsers" USING btree ("NormalizedUserName");


--
-- Name: ux_enrollments_active_participant_workshop; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX ux_enrollments_active_participant_workshop ON public.enrollments USING btree (participant_id, workshop_id) WHERE ((status)::text = 'Active'::text);


--
-- Name: AspNetRoleClaims FK_AspNetRoleClaims_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetRoleClaims"
    ADD CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserClaims FK_AspNetUserClaims_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserClaims"
    ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserLogins FK_AspNetUserLogins_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserLogins"
    ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetRoles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserRoles FK_AspNetUserRoles_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserRoles"
    ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: AspNetUserTokens FK_AspNetUserTokens_AspNetUsers_UserId; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AspNetUserTokens"
    ADD CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE;


--
-- Name: enrollments FK_enrollments_AspNetUsers_cancelled_by_user_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.enrollments
    ADD CONSTRAINT "FK_enrollments_AspNetUsers_cancelled_by_user_id" FOREIGN KEY (cancelled_by_user_id) REFERENCES public."AspNetUsers"("Id") ON DELETE SET NULL;


--
-- Name: enrollments FK_enrollments_participants_participant_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.enrollments
    ADD CONSTRAINT "FK_enrollments_participants_participant_id" FOREIGN KEY (participant_id) REFERENCES public.participants(id) ON DELETE RESTRICT;


--
-- Name: enrollments FK_enrollments_workshops_workshop_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.enrollments
    ADD CONSTRAINT "FK_enrollments_workshops_workshop_id" FOREIGN KEY (workshop_id) REFERENCES public.workshops(id) ON DELETE RESTRICT;


--
-- Name: instructors FK_instructors_AspNetUsers_user_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.instructors
    ADD CONSTRAINT "FK_instructors_AspNetUsers_user_id" FOREIGN KEY (user_id) REFERENCES public."AspNetUsers"("Id") ON DELETE RESTRICT;


--
-- Name: participants FK_participants_AspNetUsers_user_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.participants
    ADD CONSTRAINT "FK_participants_AspNetUsers_user_id" FOREIGN KEY (user_id) REFERENCES public."AspNetUsers"("Id") ON DELETE RESTRICT;


--
-- Name: workshops FK_workshops_instructors_instructor_id; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.workshops
    ADD CONSTRAINT "FK_workshops_instructors_instructor_id" FOREIGN KEY (instructor_id) REFERENCES public.instructors(id) ON DELETE RESTRICT;


--
-- PostgreSQL database dump complete
--

\unrestrict we5By7mSiMzbW92NFD8EoeedjwArNIrSgYXvpM2ImLdNhRKEKaoqj7kzygJApXk

