#pragma once
#include "Vulkan.h"
#include "Utils/DebugAssert.h"

namespace Sisyphus::Rendering::Vulkan {
	class DeviceData {
	public:
		DeviceData(vk::Device device, vk::DeviceMemory memory, size_t sizeInBytes);
		~DeviceData();

		DeviceData(const DeviceData&) = delete;
		DeviceData& operator=(const DeviceData&) = delete;
		DeviceData(DeviceData&&) = default;
		DeviceData& operator=(DeviceData&&) = default;

		void Set(const std::byte* data);
		void Get(std::byte* data) const;

		template<typename T>
		void SetData(const T& data) {
			SIS_DEBUGASSERT(sizeof(T) == sizeInBytes);

			Set(reinterpret_cast<const std::byte*>(&data));
		}

		template<typename T>
		void SetData(const T* data, size_t numObjects) {
			SIS_DEBUGASSERT(sizeof(T) * numObjects == sizeInBytes);

			Set(reinterpret_cast<const std::byte*>(&data));
		}

		template<typename T>
		void SetData(const std::vector<T>& data) {
			SIS_DEBUGASSERT(data.size() * sizeof(T) == sizeInBytes);

			Set(reinterpret_cast<const std::byte*>(data.data()));
		}

		template<typename T>
		T GetData() {
			SIS_DEBUGASSERT(sizeof(T) == sizeInBytes);

			T result;
			Get(reinterpret_cast<std::byte*>(&result));
			return result;
		}

		template<typename T>
		std::vector<T> GetDataAsVector() {
			SIS_DEBUGASSERT(sizeInBytes % sizeof(T) != 0);
			size_t numObjects = sizeInBytes / sizeof(T);
			std::vector<T> result;
			result.resize(numObjects);
			Get(reinterpret_cast<std::byte*>(result.data()));
			return result;
		}

	private:
		vk::Device device;
		vk::DeviceMemory memory;
		size_t sizeInBytes;

		std::byte* deviceData;
	};
}