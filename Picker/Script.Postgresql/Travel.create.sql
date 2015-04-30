-- Table: "Travel"

-- DROP TABLE "Travel";

CREATE TABLE "Travel"
(
  "Uri" character varying(255) NOT NULL,
  "CreatedAt" timestamp(6) with time zone,
  "UpdatedAt" timestamp(6) with time zone,
  "ProcessedAt" timestamp(6) with time zone,
  "Content" text,
  CONSTRAINT "PrimaryKey_Travel_Uri" PRIMARY KEY ("Uri")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "Travel"
  OWNER TO postgres;

-- Index: "Index_Travel"

-- DROP INDEX "Index_Travel";

CREATE INDEX "Index_Travel"
  ON "Travel"
  USING btree
  ("Uri" COLLATE pg_catalog."default", "Content" COLLATE pg_catalog."default");

