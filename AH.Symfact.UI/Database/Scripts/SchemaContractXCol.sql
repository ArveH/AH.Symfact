/*
# (Re)Create **ContractXCol** collection Add Contract schema
*/

DECLARE @x XML;

SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\Contract.xsd',
    SINGLE_BLOB
) AS x
);



IF EXISTS (SELECT name 
            FROM sys.xml_schema_collections 
            WHERE name = 'contractXCol')
    DROP XML SCHEMA COLLECTION contractXCol;

CREATE XML SCHEMA COLLECTION contractXCol AS @x;

SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\Clause.xsd',
    SINGLE_BLOB
) AS x
);

ALTER XML SCHEMA COLLECTION contractXCol ADD @x;

/*
# Add dc, meta and office
*/
SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\dc.xsd',
    SINGLE_BLOB
) AS x
);

ALTER XML SCHEMA COLLECTION contractXCol ADD @x;

SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\meta.xsd',
    SINGLE_BLOB
) AS x
);

ALTER XML SCHEMA COLLECTION contractXCol ADD @x;

SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\office.xsd',
    SINGLE_BLOB
) AS x
);

ALTER XML SCHEMA COLLECTION contractXCol ADD @x;

/*
# Add Party schema
*/
SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\Party.xsd',
    SINGLE_BLOB
) AS x
);

ALTER XML SCHEMA COLLECTION contractXCol ADD @x;

/*
# Template for nonXML
*/
SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\template.xsd',
    SINGLE_BLOB
) AS x
);

ALTER XML SCHEMA COLLECTION contractXCol ADD @x;
