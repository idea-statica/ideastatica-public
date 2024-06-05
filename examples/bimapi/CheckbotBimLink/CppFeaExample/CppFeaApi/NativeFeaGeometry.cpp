#include "pch.h"
#include "NativeFeaGeometry.h"

NativeFeaGeometry::NativeFeaGeometry()
{
	nodes = std::map<int, NativeFeaNode*>();
	nodes.insert(std::make_pair(1, new NativeFeaNode(1, 0.0, 0.0, 0.0)));
	nodes.insert(std::make_pair(2, new NativeFeaNode(2, 0.0, 0.0, 3.0)));
	nodes.insert(std::make_pair(3, new NativeFeaNode(3, 5.0, 0.0, 3.0)));

	members = std::map<int, NativeFeaMember*>();
	members.insert(std::make_pair(1, new NativeFeaMember(1, 1, 2, 1)));
	members.insert(std::make_pair(2, new NativeFeaMember(2, 2, 3, 1)));
}

NativeFeaGeometry::~NativeFeaGeometry()
{
	for (const auto& pair : nodes) {
		delete pair.second;
	}

	for (const auto& pair : members) {
		delete pair.second;
	}
}

std::vector<int> NativeFeaGeometry::GetMembersIdentifiers()
{
	std::vector<int> keys;
	for (const auto& pair : members) {
		keys.push_back(pair.first);
	}

	return keys;
}

std::vector<int> NativeFeaGeometry::GetNodesIdentifiers()
{
	std::vector<int> keys;
	for (const auto& pair : nodes) {
		keys.push_back(pair.first);
	}

	return keys;
}

NativeFeaNode* NativeFeaGeometry::GetNode(int id)
{
	return nodes[id];
}

NativeFeaMember* NativeFeaGeometry::GetMember(int id)
{
	return members[id];
}

