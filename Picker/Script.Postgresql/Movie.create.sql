-- Table: "Movie"

-- DROP TABLE "Movie";

CREATE TABLE "Movie"
(
  "Uri" character varying(255) NOT NULL,
  "CreatedAt" timestamp(6) with time zone,
  "UpdatedAt" timestamp(6) with time zone,
  "ProcessedAt" timestamp(6) with time zone,
  "Content" text,
  CONSTRAINT "PrimaryKey_Movie_Uri" PRIMARY KEY ("Uri")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "Movie"
  OWNER TO postgres;

-- Index: "Index_Movie"

-- DROP INDEX "Index_Movie";

CREATE INDEX "Index_Movie"
  ON "Movie"
  USING btree
  ("Uri" COLLATE pg_catalog."default", "Content" COLLATE pg_catalog."default");

