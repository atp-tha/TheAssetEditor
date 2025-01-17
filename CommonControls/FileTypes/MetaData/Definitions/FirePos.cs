﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonControls.FileTypes.MetaData.Definitions
{
    [MetaData("FIRE_POS", 10)]
    public class FirePos : DecodedMetaEntryBase
    {
        [MetaDataTag(5, "Measured from the unit's animroot bone, in meters.")]
        public Vector3 Position { get; set; }
    }
}


/*
 * 
 * 		<PROPERTY name="FIRE_POS" type="property_set">
			<PROPERTY name="Value" type="vector3" attribute_manipulator="p">0 2.02231622 -1.80137265</PROPERTY>
			<PROPERTY name="StartEndTime" type="float_range">0.0168807339 0.0168807339</PROPERTY>
			<PROPERTY name="Filter" type="string"></PROPERTY>
		</PROPERTY>
 * 
  "TableName": "FIRE_POS",
        "Version": 2,
        "ColumnDefinitions": [
          {
            "Name": "Version",
            "FieldReference": null,
            "TableReference": null,
            "IsKey": false,
            "IsOptional": false,
            "MaxLength": 0,
            "IsFileName": false,
            "Description": null,
            "FilenameRelativePath": null,
            "Type": "Integer"
          },
          {
            "Name": "StartTime",
            "FieldReference": null,
            "TableReference": null,
            "IsKey": false,
            "IsOptional": false,
            "MaxLength": 0,
            "IsFileName": false,
            "Description": null,
            "FilenameRelativePath": null,
            "Type": "Single"
          },
          {
            "Name": "EndTime",
            "FieldReference": null,
            "TableReference": null,
            "IsKey": false,
            "IsOptional": false,
            "MaxLength": 0,
            "IsFileName": false,
            "Description": null,
            "FilenameRelativePath": null,
            "Type": "Single"
          },
          {
            "Name": "Filter",
            "FieldReference": null,
            "TableReference": null,
            "IsKey": false,
            "IsOptional": false,
            "MaxLength": 0,
            "IsFileName": false,
            "Description": null,
            "FilenameRelativePath": null,
            "Type": "string"
          },
          {
            "Name": "Position",
            "FieldReference": null,
            "TableReference": null,
            "IsKey": false,
            "IsOptional": false,
            "MaxLength": 0,
            "IsFileName": false,
            "Description": null,
            "FilenameRelativePath": null,
            "Type": "Single"
          }
        ]
 */