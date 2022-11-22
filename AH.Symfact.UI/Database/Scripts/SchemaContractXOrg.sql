
DECLARE @x XML;
SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\contractXOrganisationalPerson.xsd',
    SINGLE_BLOB
) AS x
);

IF EXISTS (SELECT name 
            FROM sys.xml_schema_collections 
            WHERE name = 'contractXOrg')
    DROP XML SCHEMA COLLECTION contractXOrg;

CREATE XML SCHEMA COLLECTION contractXOrg AS @x;

/*
# Add OrganisationalUnit schema
*/
SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\OrganisationalUnit.xsd',
    SINGLE_BLOB
) AS x
);

ALTER XML SCHEMA COLLECTION contractXOrg ADD @x;

/*
# Add UserProfile schema
*/

SET @x = (
    SELECT * FROM OPENROWSET(
    BULK '<<<PATH>>>\Schemas\UserProfile.xsd',
    SINGLE_BLOB
) AS x
);

ALTER XML SCHEMA COLLECTION contractXOrg ADD @x;
