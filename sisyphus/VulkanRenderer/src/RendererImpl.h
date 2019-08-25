#pragma once
#include "Renderer.h"
#include "Vulkan.h"
#include "PlatformSpecific.h"
#include "DepthBuffer.h"
#include "UniformBuffer.h"
#include "Utils\ILogger.h"
#include <memory>
#include <optional>

namespace WindowCreator {
	class Window;
}

namespace Vulkan {

	class RendererImpl {
	public:
		RendererImpl(Renderer::CreateInfo ci);
		~RendererImpl(); // default

		void UpdateUniformBuffer(Renderer::UniformBufferData data);

	private:
		void InitInstance();
		void InitWindow();
		void InitSurface();
		void InitPhysicalDevice();
		void InitQueueFamilyIndex();
		void InitDevice();
		void InitCommandPool();
		void InitCommandBuffers();
		void InitFormatAndColorSpace();
		void InitSwapchain();
		void InitSwapchainImages();
		void InitImageViews();
		void InitDepthBuffer();
		void InitDescriptorSetLayout();
		void InitPipelineLayout();
		void InitDescriptorPool();
		void InitDescriptorSet();
		void InitUniformBuffer();
		void InitRenderPass();

		Renderer::CreateInfo ci;
		vk::UniqueInstance instance;
		std::unique_ptr<WindowCreator::Window> window;
		vk::UniqueSurfaceKHR surface;
		vk::PhysicalDevice physicalDevice;
		std::optional<int> queueFamilyIndex;
		vk::UniqueDevice device;
		vk::UniqueCommandPool commandPool;
		std::vector<vk::CommandBuffer> commandBuffers;
		std::optional<vk::Format> colorFormat;
		std::optional<vk::ColorSpaceKHR> colorSpace;
		vk::UniqueSwapchainKHR swapchain;
		std::vector<vk::Image> swapchainImages;
		std::vector<vk::UniqueImageView> imageViews;
		std::unique_ptr<DepthBuffer> depthBuffer;

		vk::UniqueDescriptorSetLayout descriptorSetLayout;
		vk::UniquePipelineLayout pipelineLayout;
		vk::UniqueDescriptorPool descriptorPool;
		vk::UniqueDescriptorSet descriptorSet;
		std::unique_ptr<UniformBuffer> uniformBuffer;

		vk::UniqueRenderPass renderPass;

		ILogger* logger;
	};
}