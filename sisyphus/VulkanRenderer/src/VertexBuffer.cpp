#include "VertexBuffer.h"
#include "Utils\Logger.h"
#include "MemoryUtils.h"

namespace Vulkan {
	VertexBuffer::VertexBuffer(CreateInfo inCi) :
		ci(inCi)
	{
		CreateBuffer();
		Logger::Get().Log("Buffer created!");
		AllocateMemory();
		Logger::Get().Log("Memory allocated!");
		BindMemory();
		Logger::Get().Log("Memory bound!");
	}

	DeviceData VertexBuffer::GetDeviceData()
	{
		return DeviceData(ci.device, *memory, ci.sizeInBytes);
	}

	void VertexBuffer::CreateBuffer()
	{
		vk::BufferCreateInfo bufferCreateInfo(
			{},
			ci.sizeInBytes,
			vk::BufferUsageFlagBits::eVertexBuffer
		);
		buffer = ci.device.createBufferUnique(bufferCreateInfo);
	}

	void VertexBuffer::AllocateMemory()
	{
		auto memoryRequirements = ci.device.getBufferMemoryRequirements(*buffer);
		auto memoryTypeIndex = FindMemoryType(
			ci.physicalDevice.getMemoryProperties(),
			memoryRequirements.memoryTypeBits,
			vk::MemoryPropertyFlagBits::eHostVisible | vk::MemoryPropertyFlagBits::eHostCoherent,
			&Logger::Get()
		);

		memory = ci.device.allocateMemoryUnique(vk::MemoryAllocateInfo(memoryRequirements.size, memoryTypeIndex));
	}
	void VertexBuffer::BindMemory()
	{
		BreakAssert(buffer);
		BreakAssert(memory);

		ci.device.bindBufferMemory(*buffer, *memory, 0);
	}
}
