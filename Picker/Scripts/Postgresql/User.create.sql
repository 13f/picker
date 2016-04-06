-- Table: "User"

-- DROP TABLE "User";

CREATE TABLE "User"
(
  id character varying(20) NOT NULL,
  uid character varying(63),
  "Content" text,
  "CreatedAt" timestamp(6) with time zone,
  "UpdatedAt" timestamp(6) with time zone,
  "ProcessedAt" timestamp(6) with time zone,
  type character varying(20),
  CONSTRAINT "PrimaryKey_User_id" PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "User"
  OWNER TO postgres;

-- Index: "Index_User"

-- DROP INDEX "Index_User";

CREATE INDEX "Index_User"
  ON "User"
  USING btree
  ("Content" COLLATE pg_catalog."default");

