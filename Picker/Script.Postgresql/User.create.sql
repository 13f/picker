-- Table: "User"

-- DROP TABLE "User";

CREATE TABLE "User"
(
  id character varying(20) NOT NULL,
  uid character varying(63),
  "Content" text,
  CONSTRAINT "PrimaryKey_User_id" PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "User"
  OWNER TO postgres;
