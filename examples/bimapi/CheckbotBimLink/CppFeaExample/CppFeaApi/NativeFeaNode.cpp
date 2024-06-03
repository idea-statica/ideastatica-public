#include "pch.h"
#include "NativeFeaNode.h"

NativeFeaNode::NativeFeaNode()
{
	Id = 0;
	X = 0.0;
	Y = 0.0;
	Z = 0.0;
}

NativeFeaNode::NativeFeaNode(int id, double x, double y, double z)
{
	Id = id;
	X = x;
	Y = y;
	Z = z;
}
