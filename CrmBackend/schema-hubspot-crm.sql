-- ============================================================================
--  HubS-pot-CRM  ·  PostgreSQL schema
-- ----------------------------------------------------------------------------
--  Hand-authored, standalone DDL for the CRM system.
--  Modeled on a live Bitrix24 tenant (see BITRIX24-FULL-DOCS.md).
--
--  Target:   PostgreSQL 17
--  Database: "HubS-pot-CRM"
--
--  Design notes:
--    * Money is numeric(18,2) — never float (Bitrix24's one schema flaw).
--    * Every table carries audit columns + soft delete (is_deleted).
--    * updated_at is maintained automatically by a trigger.
--    * Stage semantic (P/S/F) is stored separately from the stage itself.
--    * snake_case throughout.
-- ============================================================================

-- Run this whole file against the target database, e.g.:
--   psql -U postgres -h localhost -p 5433 -d "HubS-pot-CRM" -f schema-hubspot-crm.sql

BEGIN;

-- Needed for gen_random_uuid().
CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- ----------------------------------------------------------------------------
--  Shared trigger: keep updated_at current on every UPDATE
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS trigger AS $$
BEGIN
    NEW.updated_at := now();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
--  users
-- ============================================================================
CREATE TABLE users (
    id            uuid         PRIMARY KEY DEFAULT gen_random_uuid(),
    first_name    varchar(120) NOT NULL,
    last_name     varchar(120) NOT NULL,
    email         varchar(256) NOT NULL,
    password_hash text         NOT NULL,
    role          smallint     NOT NULL DEFAULT 2,      -- 0 admin, 1 manager, 2 agent
    is_active     boolean      NOT NULL DEFAULT true,

    -- audit + soft delete
    created_at    timestamptz  NOT NULL DEFAULT now(),
    created_by_id uuid,
    updated_at    timestamptz,
    updated_by_id uuid,
    is_deleted    boolean      NOT NULL DEFAULT false,
    deleted_at    timestamptz
);

CREATE UNIQUE INDEX ux_users_email ON users (lower(email)) WHERE is_deleted = false;
CREATE INDEX ix_users_is_deleted   ON users (is_deleted);

CREATE TRIGGER trg_users_updated_at
    BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================================
--  companies
-- ============================================================================
CREATE TABLE companies (
    id              uuid          PRIMARY KEY DEFAULT gen_random_uuid(),
    name            varchar(256)  NOT NULL,
    industry        varchar(120),
    website         varchar(256),
    phone           varchar(40),
    email           varchar(256),
    address_city    varchar(120),
    address_country varchar(120),
    annual_revenue  numeric(18,2),
    employee_count  integer,

    -- responsible person
    assigned_to_id  uuid,

    created_at      timestamptz   NOT NULL DEFAULT now(),
    created_by_id   uuid,
    updated_at      timestamptz,
    updated_by_id   uuid,
    is_deleted      boolean       NOT NULL DEFAULT false,
    deleted_at      timestamptz,

    CONSTRAINT fk_companies_assigned_to
        FOREIGN KEY (assigned_to_id) REFERENCES users (id) ON DELETE RESTRICT
);

CREATE INDEX ix_companies_name        ON companies (name);
CREATE INDEX ix_companies_assigned_to ON companies (assigned_to_id);
CREATE INDEX ix_companies_is_deleted  ON companies (is_deleted);

CREATE TRIGGER trg_companies_updated_at
    BEFORE UPDATE ON companies
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================================
--  contacts
-- ============================================================================
CREATE TABLE contacts (
    id             uuid          PRIMARY KEY DEFAULT gen_random_uuid(),
    first_name     varchar(120)  NOT NULL,
    last_name      varchar(120)  NOT NULL,
    email          varchar(256),
    phone          varchar(40),
    position       varchar(160),

    -- a contact may belong to a company (nullable)
    company_id     uuid,
    assigned_to_id uuid,

    created_at     timestamptz   NOT NULL DEFAULT now(),
    created_by_id  uuid,
    updated_at     timestamptz,
    updated_by_id  uuid,
    is_deleted     boolean       NOT NULL DEFAULT false,
    deleted_at     timestamptz,

    CONSTRAINT fk_contacts_company
        FOREIGN KEY (company_id) REFERENCES companies (id) ON DELETE SET NULL,
    CONSTRAINT fk_contacts_assigned_to
        FOREIGN KEY (assigned_to_id) REFERENCES users (id) ON DELETE RESTRICT
);

CREATE INDEX ix_contacts_company     ON contacts (company_id);
CREATE INDEX ix_contacts_assigned_to ON contacts (assigned_to_id);
CREATE INDEX ix_contacts_email       ON contacts (email);
CREATE INDEX ix_contacts_is_deleted  ON contacts (is_deleted);

CREATE TRIGGER trg_contacts_updated_at
    BEFORE UPDATE ON contacts
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================================
--  applications   (the deal / pipeline entity)
-- ============================================================================
CREATE TABLE applications (
    id              uuid          PRIMARY KEY DEFAULT gen_random_uuid(),
    title           varchar(256)  NOT NULL,

    -- pipeline position
    stage           smallint      NOT NULL DEFAULT 0,   -- 0 New … 5 Won, 6 Lost
    stage_semantic  smallint      NOT NULL DEFAULT 0,   -- 0 Process, 1 Success, 2 Failure
    is_closed       boolean       NOT NULL DEFAULT false,

    -- money (never float)
    amount          numeric(18,2) NOT NULL DEFAULT 0,
    currency_code   char(3)       NOT NULL DEFAULT 'USD',
    probability     integer       NOT NULL DEFAULT 0,

    begin_date      timestamptz,
    close_date      timestamptz,

    -- relationships
    contact_id      uuid,
    company_id      uuid,
    assigned_to_id  uuid,

    created_at      timestamptz   NOT NULL DEFAULT now(),
    created_by_id   uuid,
    updated_at      timestamptz,
    updated_by_id   uuid,
    is_deleted      boolean       NOT NULL DEFAULT false,
    deleted_at      timestamptz,

    CONSTRAINT fk_applications_contact
        FOREIGN KEY (contact_id) REFERENCES contacts (id) ON DELETE SET NULL,
    CONSTRAINT fk_applications_company
        FOREIGN KEY (company_id) REFERENCES companies (id) ON DELETE SET NULL,
    CONSTRAINT fk_applications_assigned_to
        FOREIGN KEY (assigned_to_id) REFERENCES users (id) ON DELETE RESTRICT,

    -- a deal cannot be "closed" while its semantic is still "in process"
    CONSTRAINT ck_applications_stage_semantic
        CHECK ((stage_semantic = 0) = (is_closed = false))
);

CREATE INDEX ix_applications_stage        ON applications (stage, is_closed);
CREATE INDEX ix_applications_contact      ON applications (contact_id);
CREATE INDEX ix_applications_company      ON applications (company_id);
CREATE INDEX ix_applications_assigned_to  ON applications (assigned_to_id);
CREATE INDEX ix_applications_is_deleted   ON applications (is_deleted);

CREATE TRIGGER trg_applications_updated_at
    BEFORE UPDATE ON applications
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================================
--  activities   (scheduled work: call, meeting, task)
-- ============================================================================
CREATE TABLE activities (
    id             uuid          PRIMARY KEY DEFAULT gen_random_uuid(),
    subject        varchar(256)  NOT NULL,
    description    text,
    type           smallint      NOT NULL DEFAULT 3,   -- 1 meeting, 2 call, 3 task, 4 email, 5 general, 6 user-action
    status         smallint      NOT NULL DEFAULT 1,   -- 1 pending, 2 completed, 3 auto-completed

    start_time     timestamptz,
    deadline       timestamptz,
    is_completed   boolean       NOT NULL DEFAULT false,

    -- owner links
    contact_id     uuid,
    application_id uuid,
    assigned_to_id uuid,

    created_at     timestamptz   NOT NULL DEFAULT now(),
    created_by_id  uuid,
    updated_at     timestamptz,
    updated_by_id  uuid,
    is_deleted     boolean       NOT NULL DEFAULT false,
    deleted_at     timestamptz,

    CONSTRAINT fk_activities_contact
        FOREIGN KEY (contact_id) REFERENCES contacts (id) ON DELETE CASCADE,
    CONSTRAINT fk_activities_application
        FOREIGN KEY (application_id) REFERENCES applications (id) ON DELETE SET NULL,
    CONSTRAINT fk_activities_assigned_to
        FOREIGN KEY (assigned_to_id) REFERENCES users (id) ON DELETE RESTRICT
);

-- powers the "overdue" / "due today" counters
CREATE INDEX ix_activities_due         ON activities (assigned_to_id, deadline, is_completed);
CREATE INDEX ix_activities_contact     ON activities (contact_id);
CREATE INDEX ix_activities_application ON activities (application_id);
CREATE INDEX ix_activities_is_deleted  ON activities (is_deleted);

CREATE TRIGGER trg_activities_updated_at
    BEFORE UPDATE ON activities
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================================
--  messages   (immutable communication log)
-- ============================================================================
CREATE TABLE messages (
    id            uuid          PRIMARY KEY DEFAULT gen_random_uuid(),
    channel       smallint      NOT NULL DEFAULT 0,   -- 0 email,1 sms,2 whatsapp,3 telegram,4 livechat,5 call
    direction     smallint      NOT NULL DEFAULT 0,   -- 0 inbound, 1 outbound
    subject       varchar(256),
    body          text          NOT NULL,
    sent_at       timestamptz   NOT NULL DEFAULT now(),
    is_read       boolean       NOT NULL DEFAULT false,

    -- the contact this message is with (required)
    contact_id    uuid          NOT NULL,
    -- the user who handled it (nullable for inbound machine messages)
    user_id       uuid,

    created_at    timestamptz   NOT NULL DEFAULT now(),
    created_by_id uuid,
    updated_at    timestamptz,
    updated_by_id uuid,
    is_deleted    boolean       NOT NULL DEFAULT false,
    deleted_at    timestamptz,

    CONSTRAINT fk_messages_contact
        FOREIGN KEY (contact_id) REFERENCES contacts (id) ON DELETE CASCADE,
    CONSTRAINT fk_messages_user
        FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE SET NULL
);

CREATE INDEX ix_messages_contact_sent ON messages (contact_id, sent_at);
CREATE INDEX ix_messages_channel      ON messages (channel);
CREATE INDEX ix_messages_is_deleted   ON messages (is_deleted);

CREATE TRIGGER trg_messages_updated_at
    BEFORE UPDATE ON messages
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

COMMIT;

-- ============================================================================
--  End of schema. 6 tables · 11 foreign keys · 20+ indexes · auto updated_at.
-- ============================================================================
