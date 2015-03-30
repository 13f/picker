-- Table: "Book"

-- DROP TABLE "Book";

CREATE TABLE "Book"
(
  "Uri" character varying(255) NOT NULL,
  "CreatedAt" timestamp(6) with time zone,
  "UpdatedAt" timestamp(6) with time zone,
  "ProcessedAt" timestamp(6) with time zone,
  "Content" text,
  CONSTRAINT "PrimaryKey_Book_Uri" PRIMARY KEY ("Uri")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "Book"
  OWNER TO postgres;
