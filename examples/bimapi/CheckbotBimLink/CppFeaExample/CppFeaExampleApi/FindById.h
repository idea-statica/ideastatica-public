#pragma once

#include <algorithm>
#include <stdexcept>
#include <string>
#include <vector>

namespace CppFeaExampleApi
{
	/// <summary>
	/// The item with the given id. Throws std::out_of_range when there is none.
	/// </summary>
	template <typename T>
	const T& FindById(const std::vector<T>& items, int id)
	{
		auto found = std::find_if(items.begin(), items.end(), [id](const T& item) { return item.Id == id; });
		if (found == items.end())
		{
			throw std::out_of_range("No item has the id " + std::to_string(id) + ".");
		}

		return *found;
	}
}
