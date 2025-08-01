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

from pydantic import BaseModel, ConfigDict, Field
from typing import Any, ClassVar, Dict, List, Optional
from ideastatica_connection_api.models.poly_line2_d import PolyLine2D
from typing import Optional, Set
from typing_extensions import Self

class Region2D(BaseModel):
    """
    Represents a region in two-dimensional space included outline (border) and openings.
    """ # noqa: E501
    outline: Optional[PolyLine2D] = None
    openings: Optional[List[PolyLine2D]] = Field(default=None, description="Gets or sets the list of openings in the Region2D.")
    __properties: ClassVar[List[str]] = ["outline", "openings"]

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
        """Create an instance of Region2D from a JSON string"""
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
        # override the default output from pydantic by calling `to_dict()` of outline
        if self.outline:
            _dict['outline'] = self.outline.to_dict()
        # override the default output from pydantic by calling `to_dict()` of each item in openings (list)
        _items = []
        if self.openings:
            for _item_openings in self.openings:
                if _item_openings:
                    _items.append(_item_openings.to_dict())
            _dict['openings'] = _items
        # set to None if openings (nullable) is None
        # and model_fields_set contains the field
        if self.openings is None and "openings" in self.model_fields_set:
            _dict['openings'] = None

        return _dict

    @classmethod
    def from_dict(cls, obj: Optional[Dict[str, Any]]) -> Optional[Self]:
        """Create an instance of Region2D from a dict"""
        if obj is None:
            return None

        if not isinstance(obj, dict):
            return cls.model_validate(obj)

        _obj = cls.model_validate({
            "outline": PolyLine2D.from_dict(obj["outline"]) if obj.get("outline") is not None else None,
            "openings": [PolyLine2D.from_dict(_item) for _item in obj["openings"]] if obj.get("openings") is not None else None
        })
        return _obj


