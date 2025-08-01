# coding: utf-8

"""
    Connection Rest API 2.0

    IDEA StatiCa Connection API, used for the automated design and calculation of steel connections.

    The version of the OpenAPI document: 2.0
    Contact: info@ideastatica.com
    Generated by OpenAPI Generator (https://openapi-generator.tech)

    Do not edit the class manually.
"""  # noqa: E501


from __future__ import annotations
import pprint
import re  # noqa: F401
import json

from pydantic import BaseModel, ConfigDict, StrictFloat, StrictInt
from typing import Any, ClassVar, Dict, List, Optional, Union
from ideastatica_connection_api.models.i_group import IGroup
from typing import Optional, Set
from typing_extensions import Self

class DrawData(BaseModel):
    """
    DrawData
    """ # noqa: E501
    groups: Optional[List[IGroup]] = None
    vertices: Optional[List[Union[StrictFloat, StrictInt]]] = None
    normals: Optional[List[Union[StrictFloat, StrictInt]]] = None
    __properties: ClassVar[List[str]] = ["groups", "vertices", "normals"]

    model_config = ConfigDict(
        populate_by_name=True,
        validate_assignment=True,
        protected_namespaces=(),
    )


    def to_str(self) -> str:
        """Returns the string representation of the model using alias"""
        return pprint.pformat(self.model_dump(by_alias=True))

    def to_json(self) -> str:
        """Returns the JSON representation of the model using alias"""
        # TODO: pydantic v2: use .model_dump_json(by_alias=True, exclude_unset=True) instead
        return json.dumps(self.to_dict())

    @classmethod
    def from_json(cls, json_str: str) -> Optional[Self]:
        """Create an instance of DrawData from a JSON string"""
        return cls.from_dict(json.loads(json_str))

    def to_dict(self) -> Dict[str, Any]:
        """Return the dictionary representation of the model using alias.

        This has the following differences from calling pydantic's
        `self.model_dump(by_alias=True)`:

        * `None` is only added to the output dict for nullable fields that
          were set at model initialization. Other fields with value `None`
          are ignored.
        """
        excluded_fields: Set[str] = set([
        ])

        _dict = self.model_dump(
            by_alias=True,
            exclude=excluded_fields,
            exclude_none=True,
        )
        # override the default output from pydantic by calling `to_dict()` of each item in groups (list)
        _items = []
        if self.groups:
            for _item_groups in self.groups:
                if _item_groups:
                    _items.append(_item_groups.to_dict())
            _dict['groups'] = _items
        # set to None if groups (nullable) is None
        # and model_fields_set contains the field
        if self.groups is None and "groups" in self.model_fields_set:
            _dict['groups'] = None

        # set to None if vertices (nullable) is None
        # and model_fields_set contains the field
        if self.vertices is None and "vertices" in self.model_fields_set:
            _dict['vertices'] = None

        # set to None if normals (nullable) is None
        # and model_fields_set contains the field
        if self.normals is None and "normals" in self.model_fields_set:
            _dict['normals'] = None

        return _dict

    @classmethod
    def from_dict(cls, obj: Optional[Dict[str, Any]]) -> Optional[Self]:
        """Create an instance of DrawData from a dict"""
        if obj is None:
            return None

        if not isinstance(obj, dict):
            return cls.model_validate(obj)

        _obj = cls.model_validate({
            "groups": [IGroup.from_dict(_item) for _item in obj["groups"]] if obj.get("groups") is not None else None,
            "vertices": obj.get("vertices"),
            "normals": obj.get("normals")
        })
        return _obj


