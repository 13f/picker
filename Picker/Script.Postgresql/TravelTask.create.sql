-- Table: "TravelTask"

-- DROP TABLE "TravelTask";

CREATE TABLE "TravelTask"
(
  "ApiUri" character varying(255) NOT NULL,
  "ProcessedAt" timestamp(6) with time zone,
  CONSTRAINT "PrimaryKey_TravelTask_ApiUri" PRIMARY KEY ("ApiUri")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "TravelTask"
  OWNER TO postgres;
