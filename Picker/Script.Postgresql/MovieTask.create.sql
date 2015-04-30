-- Table: "MovieTask"

-- DROP TABLE "MovieTask";

CREATE TABLE "MovieTask"
(
  "ApiUri" character varying(255) NOT NULL,
  "ProcessedAt" timestamp(6) with time zone,
  CONSTRAINT "PrimaryKey_MovieTask_ApiUri" PRIMARY KEY ("ApiUri")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "MovieTask"
  OWNER TO postgres;

-- Index: "Index_MovieTask"

-- DROP INDEX "Index_MovieTask";

CREATE INDEX "Index_MovieTask"
  ON "MovieTask"
  USING btree
  ("ApiUri" COLLATE pg_catalog."default");

