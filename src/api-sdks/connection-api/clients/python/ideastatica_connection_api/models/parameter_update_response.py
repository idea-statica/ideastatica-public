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

from pydantic import BaseModel, ConfigDict, Field, StrictBool
from typing import Any, ClassVar, Dict, List, Optional
from ideastatica_connection_api.models.idea_parameter import IdeaParameter
from typing import Optional, Set
from typing_extensions import Self

class ParameterUpdateResponse(BaseModel):
    """
    ParameterUpdateResponse
    """ # noqa: E501
    set_to_model: Optional[StrictBool] = Field(default=None, alias="setToModel")
    parameters: Optional[List[IdeaParameter]] = None
    failed_validations: Optional[List[Dict[str, Any]]] = Field(default=None, alias="failedValidations")
    __properties: ClassVar[List[str]] = ["setToModel", "parameters", "failedValidations"]

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
        """Create an instance of ParameterUpdateResponse from a JSON string"""
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
        # override the default output from pydantic by calling `to_dict()` of each item in parameters (list)
        _items = []
        if self.parameters:
            for _item_parameters in self.parameters:
                if _item_parameters:
                    _items.append(_item_parameters.to_dict())
            _dict['parameters'] = _items
        # set to None if parameters (nullable) is None
        # and model_fields_set contains the field
        if self.parameters is None and "parameters" in self.model_fields_set:
            _dict['parameters'] = None

        # set to None if failed_validations (nullable) is None
        # and model_fields_set contains the field
        if self.failed_validations is None and "failed_validations" in self.model_fields_set:
            _dict['failedValidations'] = None

        return _dict

    @classmethod
    def from_dict(cls, obj: Optional[Dict[str, Any]]) -> Optional[Self]:
        """Create an instance of ParameterUpdateResponse from a dict"""
        if obj is None:
            return None

        if not isinstance(obj, dict):
            return cls.model_validate(obj)

        _obj = cls.model_validate({
            "setToModel": obj.get("setToModel"),
            "parameters": [IdeaParameter.from_dict(_item) for _item in obj["parameters"]] if obj.get("parameters") is not None else None,
            "failedValidations": obj.get("failedValidations")
        })
        return _obj


