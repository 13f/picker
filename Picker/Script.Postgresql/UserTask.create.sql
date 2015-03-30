-- Table: "UserTask"

-- DROP TABLE "UserTask";

CREATE TABLE "UserTask"
(
  "ProcessedAt" timestamp(6) with time zone,
  id character varying(20) NOT NULL,
  uid character varying(63),
  CONSTRAINT "PrimaryKey_UserTask_id" PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "UserTask"
  OWNER TO postgres;
