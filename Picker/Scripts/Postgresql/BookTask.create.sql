﻿-- Table: "BookTask"

-- DROP TABLE "BookTask";

CREATE TABLE "BookTask"
(
  "ApiUri" character varying(255) NOT NULL,
  "ProcessedAt" timestamp(6) with time zone,
  CONSTRAINT "PrimaryKey_BookTask_ApiUri" PRIMARY KEY ("ApiUri")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "BookTask"
  OWNER TO postgres;

-- Index: "Index_BookTask"

-- DROP INDEX "Index_BookTask";

CREATE INDEX "Index_BookTask"
  ON "BookTask"
  USING btree
  ("ApiUri" COLLATE pg_catalog."default");

