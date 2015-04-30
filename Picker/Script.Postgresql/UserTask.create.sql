-- Table: "UserTask"

-- DROP TABLE "UserTask";

CREATE TABLE "UserTask"
(
  "ProcessedAt" timestamp(6) with time zone,
  id character varying(20) NOT NULL,
  uid character varying(63),
  "BooksProcessedAt" timestamp(6) with time zone,
  "TravelProcessedAt" timestamp(6) with time zone,
  CONSTRAINT "PrimaryKey_UserTask_id" PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "UserTask"
  OWNER TO postgres;

-- Index: "Index_UserTask"

-- DROP INDEX "Index_UserTask";

CREATE INDEX "Index_UserTask"
  ON "UserTask"
  USING btree
  (id COLLATE pg_catalog."default", uid COLLATE pg_catalog."default");

