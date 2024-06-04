#pragma once
#include "CppFeaApiDll.h"
#include "NativeFeaMember.h"
#include "NativeFeaNode.h"
#include <iostream>
#include <vector>
#include <map>

class CPPFEAAPIDLL_EXPORT NativeFeaGeometry
{
private:
		std::map<int, NativeFeaNode*> nodes;
		std::map<int, NativeFeaMember*> members;

	public:
		NativeFeaGeometry();
		~NativeFeaGeometry();

		std::vector<int> GetMembersIdentifiers();
		std::vector<int> GetNodesIdentifiers();
		NativeFeaNode* GetNode(int id);
		NativeFeaMember* GetMember(int id);
};

