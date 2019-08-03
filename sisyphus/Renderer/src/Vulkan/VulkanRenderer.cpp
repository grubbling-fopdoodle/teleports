#include <vector>
#include <string>
#include <iostream>
#include <sstream>
#include "VulkanRenderer.h"
#include "WindowCreator/WindowCreator.h"
#include "Utils\BreakAssert.h"

namespace wc = WindowCreator;

void InspectDevice(const vk::PhysicalDevice& physicalDevice) {
	auto queueFamilyProperties = physicalDevice.getQueueFamilyProperties();
	std::cout << "Queue Families:\n";
	int index = 1;
	for (auto&& props : queueFamilyProperties) {
		auto flags = props.queueFlags;
		std::cout << "\t#" << index << ": " << vk::to_string(flags) << " Count: " << props.queueCount << std::endl;
		index++;
	}
}

std::optional<int> FindGraphicsQueueFamilyIndex(vk::PhysicalDevice& physicalDevice, vk::SurfaceKHR& surface) {
	auto queueFamilyProperties = physicalDevice.getQueueFamilyProperties();
	for (int i = 0; i < queueFamilyProperties.size(); i++) {
		if ((queueFamilyProperties[i].queueFlags & vk::QueueFlagBits::eGraphics) &&
			physicalDevice.getSurfaceSupportKHR(i, surface)) {
			return i;
		}
	}
	return std::nullopt;
}

VulkanRenderer::VulkanRenderer(CreateInfo ci):
	ci(ci),
	instance(nullptr),
	window(nullptr),
	surface(nullptr),
	physicalDevice(nullptr),
	queueFamilyIndex(std::nullopt),
	device(nullptr),
	commandPool(nullptr),
	swapchain(nullptr)
{
	InitInstance();
	InitWindow();
	InitSurface();
	InitPhysicalDevice();
	InitQueueFamilyIndex();
	InitDevice();
	InitCommandPool();
	InitCommandBuffers();
	InitSwapchain();
	InitSwapchainImages();
}

VulkanRenderer::~VulkanRenderer() = default;

void VulkanRenderer::InitInstance()
{
	vk::ApplicationInfo applicationInfo(
		"Vulkan App",
		VK_MAKE_VERSION(1, 0, 0),
		"Sisyphus",
		VK_MAKE_VERSION(1, 0, 0),
		VK_API_VERSION_1_0
	);

	std::vector<const char*> instanceExtensionNames = PlatformSpecific::GetInstanceExtensionNames();
	instanceExtensionNames.push_back(VK_KHR_SURFACE_EXTENSION_NAME);

	vk::InstanceCreateInfo instanceCreateInfo(
		{},
		&applicationInfo,
		0,
		nullptr,
		static_cast<uint32_t>(instanceExtensionNames.size()),
		instanceExtensionNames.data()
	);

	instance = vk::createInstanceUnique(instanceCreateInfo);
}

void VulkanRenderer::InitWindow()
{
	wc::WindowCreator windowCreator;
	window = windowCreator.Create({ wc::Platform::Windows, ci.windowWidth, ci.windowHeight });
}

void VulkanRenderer::InitSurface()
{
	BreakAssert(instance);
	BreakAssert(window);
	surface = window->GetVulkanSurface(instance.get());
}

void VulkanRenderer::InitPhysicalDevice()
{
	BreakAssert(instance);

	auto physicalDevices = instance->enumeratePhysicalDevices();
	if (physicalDevices.empty()) {
		throw::std::runtime_error("No physical devices supporting Vulkan");
	}

	physicalDevice = physicalDevices[0];
	std::cout << "Creating a Vulkan Device from " << physicalDevice.getProperties().deviceName << std::endl;
	InspectDevice(physicalDevice);
}

void VulkanRenderer::InitQueueFamilyIndex()
{
	BreakAssert(physicalDevice);
	BreakAssert(surface);
	queueFamilyIndex = FindGraphicsQueueFamilyIndex(physicalDevice, *surface);
	if (queueFamilyIndex == -1) {
		throw std::runtime_error("Graphics queue not found in the device");
	}
	std::cout << "Choosing queue family #" << queueFamilyIndex.value() << std::endl;
}

void VulkanRenderer::InitDevice()
{
	BreakAssert(physicalDevice);
	BreakAssert(queueFamilyIndex);
	
	vk::DeviceQueueCreateInfo deviceQueueCreateInfo(
		{},
		queueFamilyIndex.value(),
		1
	);

	std::vector<const char*> deviceExtensionNames;
	deviceExtensionNames.push_back(VK_KHR_SWAPCHAIN_EXTENSION_NAME);

	vk::DeviceCreateInfo deviceCreateInfo(
		{},
		1,
		&deviceQueueCreateInfo,
		0,
		nullptr,
		static_cast<uint32_t>(deviceExtensionNames.size()),
		deviceExtensionNames.data()
	);
	device = physicalDevice.createDeviceUnique(deviceCreateInfo);
}

void VulkanRenderer::InitCommandPool()
{
	BreakAssert(queueFamilyIndex);
	BreakAssert(device);

	vk::CommandPoolCreateInfo commandPoolCreateInfo(
		{},
		queueFamilyIndex.value()
	);
	commandPool = device->createCommandPoolUnique(commandPoolCreateInfo);
}

void VulkanRenderer::InitCommandBuffers()
{
	BreakAssert(device);
	BreakAssert(commandPool);

	vk::CommandBufferAllocateInfo commandBufferAllocateInfo(
		*commandPool,
		vk::CommandBufferLevel::ePrimary,
		1
	);
	commandBuffers = device->allocateCommandBuffers(commandBufferAllocateInfo);
}

void VulkanRenderer::InitSwapchain()
{
	BreakAssert(physicalDevice);
	BreakAssert(surface);
	BreakAssert(queueFamilyIndex);

	constexpr int desiredMinImageCount = 3; // triple buffering
	auto surfaceCapabilites = physicalDevice.getSurfaceCapabilitiesKHR(*surface);
	std::cout << "Surface minImageCount: " << surfaceCapabilites.minImageCount << std::endl;
	std::cout << "Surface maxImageCount: " << surfaceCapabilites.maxImageCount << std::endl;
	if (
		surfaceCapabilites.minImageCount > desiredMinImageCount ||
		surfaceCapabilites.maxImageCount < desiredMinImageCount) 
	{
		throw std::runtime_error("Surface does not support three image buffers");
	}


	constexpr vk::Format desiredFormat = vk::Format::eB8G8R8A8Srgb;
	constexpr vk::ColorSpaceKHR desiredColorSpace = vk::ColorSpaceKHR::eSrgbNonlinear;
	bool formatFound = false;

	auto surfaceFormats = physicalDevice.getSurfaceFormatsKHR(*surface);
	std::cout << "Surface formats:\n";
	for (int i = 0; i < surfaceFormats.size(); i++) {
		const auto& format = surfaceFormats[i];
		std::cout << "\t#" << i << ": \n";
		std::cout << "\t\tFormat: " << vk::to_string(format.format) << std::endl;
		std::cout << "\t\tColor Space: " << vk::to_string(format.colorSpace) << std::endl;

		if (format.format == desiredFormat && format.colorSpace == desiredColorSpace) {
			formatFound = true;
			break;
		}
	}
	if (!formatFound) {
		std::stringstream ss;
		ss << "Unable to find desired format: " << vk::to_string(desiredFormat)
			<< " and color space: " << vk::to_string(desiredColorSpace);
		throw std::runtime_error(ss.str());
	}


	std::cout << "Surface extent: (w: " << surfaceCapabilites.currentExtent.width <<
		", h: " << surfaceCapabilites.currentExtent.height << ")\n";

	if (!(surfaceCapabilites.supportedTransforms & vk::SurfaceTransformFlagBitsKHR::eIdentity)) {
		throw std::runtime_error("Identity surface transform not supported");
	}

	std::cout << "Supported composite alpha: " << vk::to_string(surfaceCapabilites.supportedCompositeAlpha) << "\n";
	if (!(surfaceCapabilites.supportedCompositeAlpha & vk::CompositeAlphaFlagBitsKHR::eOpaque)) {
		throw std::runtime_error("Surface opaque composite alpha mode not supported");
	}

	auto desiredPresentMode = vk::PresentModeKHR::eFifoRelaxed;
	auto supportedPresentModes = physicalDevice.getSurfacePresentModesKHR(surface.get());
	bool modeFound = false;
	std::cout << "Supported present modes: ";
	for (auto&& mode : supportedPresentModes) {
		std::cout << vk::to_string(mode) << "\n";
		if (mode == desiredPresentMode) {
			modeFound = true;
		}
	}
	if (!modeFound) {
		throw std::runtime_error("Present mode " + vk::to_string(desiredPresentMode) + " not supported by GPU");
	}
	
	vk::SwapchainCreateInfoKHR swapchainCreateInfo(
		{},
		*surface,
		desiredMinImageCount,
		desiredFormat,
		desiredColorSpace,
		vk::Extent2D{ci.windowWidth, ci.windowHeight},
		1,
		vk::ImageUsageFlagBits::eColorAttachment,
		vk::SharingMode::eExclusive,
		0,
		nullptr,
		vk::SurfaceTransformFlagBitsKHR::eIdentity,
		vk::CompositeAlphaFlagBitsKHR::eOpaque,
		desiredPresentMode,
		false,
		nullptr
	);

	swapchain = device->createSwapchainKHRUnique(swapchainCreateInfo);
	std::cout << "Swapchain initialized!\n";

	auto swapchainImages = device->getSwapchainImagesKHR(*swapchain);	
}

void VulkanRenderer::InitSwapchainImages()
{
	BreakAssert(swapchain);
	swapchainImages = device->getSwapchainImagesKHR(*swapchain);

	std::cout << swapchainImages.size() << " swapchain images initialized!\n";	
}
