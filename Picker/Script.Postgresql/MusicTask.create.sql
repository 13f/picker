-- Table: "MusicTask"

-- DROP TABLE "MusicTask";

CREATE TABLE "MusicTask"
(
  "ApiUri" character varying(255) NOT NULL,
  "ProcessedAt" timestamp(6) with time zone,
  CONSTRAINT "PrimaryKey_MusicTask_ApiUri" PRIMARY KEY ("ApiUri")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "MusicTask"
  OWNER TO postgres;

-- Index: "Index_MusicTask"

-- DROP INDEX "Index_MusicTask";

CREATE INDEX "Index_MusicTask"
  ON "MusicTask"
  USING btree
  ("ApiUri" COLLATE pg_catalog."default");

