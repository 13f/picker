-- Table: "Music"

-- DROP TABLE "Music";

CREATE TABLE "Music"
(
  "Uri" character varying(255) NOT NULL,
  "CreatedAt" timestamp(6) with time zone,
  "UpdatedAt" timestamp(6) with time zone,
  "ProcessedAt" timestamp(6) with time zone,
  "Content" text,
  CONSTRAINT "PrimaryKey_Music_Uri" PRIMARY KEY ("Uri")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "Music"
  OWNER TO postgres;

-- Index: "Index_Music"

-- DROP INDEX "Index_Music";

CREATE INDEX "Index_Music"
  ON "Music"
  USING btree
  ("Uri" COLLATE pg_catalog."default", "Content" COLLATE pg_catalog."default");

