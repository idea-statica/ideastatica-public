#pragma once
#include "SafProviderDll.h"

class SAFPROVIDERDLL_EXPORTS SafProviderBase
{
public:
  // Define a pure virtual function by assigning 0 to it
  virtual void someFunction() = 0;

  // Virtual destructor for proper cleanup
  virtual ~SafProviderBase() = default;
};

