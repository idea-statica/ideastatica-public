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

from pydantic import BaseModel, ConfigDict, Field, StrictFloat, StrictInt
from typing import Any, ClassVar, Dict, List, Optional, Union
from ideastatica_connection_api.models.con_aligned_plate import ConAlignedPlate
from ideastatica_connection_api.models.con_member_alignment_type_enum import ConMemberAlignmentTypeEnum
from ideastatica_connection_api.models.con_member_placement_definition_type_enum import ConMemberPlacementDefinitionTypeEnum
from ideastatica_connection_api.models.vector3_d import Vector3D
from typing import Optional, Set
from typing_extensions import Self

class ConMemberPosition(BaseModel):
    """
    ConMemberPosition
    """ # noqa: E501
    defined_by: Optional[ConMemberPlacementDefinitionTypeEnum] = Field(default=None, alias="definedBy")
    axis_x: Optional[Vector3D] = Field(default=None, alias="axisX")
    beta_direction: Optional[Union[StrictFloat, StrictInt]] = Field(default=None, alias="betaDirection")
    gama_pitch: Optional[Union[StrictFloat, StrictInt]] = Field(default=None, alias="gamaPitch")
    alpha_rotation: Optional[Union[StrictFloat, StrictInt]] = Field(default=None, alias="alphaRotation")
    offset_ex: Optional[Union[StrictFloat, StrictInt]] = Field(default=None, alias="offsetEx")
    offset_ey: Optional[Union[StrictFloat, StrictInt]] = Field(default=None, alias="offsetEy")
    offset_ez: Optional[Union[StrictFloat, StrictInt]] = Field(default=None, alias="offsetEz")
    align: Optional[ConMemberAlignmentTypeEnum] = None
    aligned_plate: Optional[ConAlignedPlate] = Field(default=None, alias="alignedPlate")
    related_plate: Optional[ConAlignedPlate] = Field(default=None, alias="relatedPlate")
    __properties: ClassVar[List[str]] = ["definedBy", "axisX", "betaDirection", "gamaPitch", "alphaRotation", "offsetEx", "offsetEy", "offsetEz", "align", "alignedPlate", "relatedPlate"]

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
        """Create an instance of ConMemberPosition from a JSON string"""
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
        # override the default output from pydantic by calling `to_dict()` of axis_x
        if self.axis_x:
            _dict['axisX'] = self.axis_x.to_dict()
        # override the default output from pydantic by calling `to_dict()` of aligned_plate
        if self.aligned_plate:
            _dict['alignedPlate'] = self.aligned_plate.to_dict()
        # override the default output from pydantic by calling `to_dict()` of related_plate
        if self.related_plate:
            _dict['relatedPlate'] = self.related_plate.to_dict()
        # set to None if beta_direction (nullable) is None
        # and model_fields_set contains the field
        if self.beta_direction is None and "beta_direction" in self.model_fields_set:
            _dict['betaDirection'] = None

        # set to None if gama_pitch (nullable) is None
        # and model_fields_set contains the field
        if self.gama_pitch is None and "gama_pitch" in self.model_fields_set:
            _dict['gamaPitch'] = None

        return _dict

    @classmethod
    def from_dict(cls, obj: Optional[Dict[str, Any]]) -> Optional[Self]:
        """Create an instance of ConMemberPosition from a dict"""
        if obj is None:
            return None

        if not isinstance(obj, dict):
            return cls.model_validate(obj)

        _obj = cls.model_validate({
            "definedBy": obj.get("definedBy"),
            "axisX": Vector3D.from_dict(obj["axisX"]) if obj.get("axisX") is not None else None,
            "betaDirection": obj.get("betaDirection"),
            "gamaPitch": obj.get("gamaPitch"),
            "alphaRotation": obj.get("alphaRotation"),
            "offsetEx": obj.get("offsetEx"),
            "offsetEy": obj.get("offsetEy"),
            "offsetEz": obj.get("offsetEz"),
            "align": obj.get("align"),
            "alignedPlate": ConAlignedPlate.from_dict(obj["alignedPlate"]) if obj.get("alignedPlate") is not None else None,
            "relatedPlate": ConAlignedPlate.from_dict(obj["relatedPlate"]) if obj.get("relatedPlate") is not None else None
        })
        return _obj


