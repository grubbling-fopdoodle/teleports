#pragma once
#include "ECS\Component.h"
#include "ECS\DependencyGraph.h"
#include "Utils/Throw.h"
#include "Utils/Logger.h"
#include "Utils/StringUtils.h"
#include "uuid.h"
#include <vector>
#include <memory>
#include <unordered_map>

namespace Sisyphus::ECS {
	class Entity {
	public:

		Entity() = default;
		Entity(const Entity&) = delete;
		Entity(Entity&&) = delete;
		Entity& operator=(const Entity&) = delete;
		Entity& operator=(Entity&&) = delete;
		~Entity();

		template<Component T, typename... ConstructorArgs>
		void InitComponent(ConstructorArgs... args) {
			uuids::uuid type = T::TypeId();
			SIS_THROWASSERT_MSG(!components.contains(type), T::ClassName() + " already exists.");
			CheckDependencies<T>();

			if (!knownComponentTypes.contains(type)) {
				UpdateSubscriberLists<T>();
				dependencyGraph.Add<T>();
			}

			std::unique_ptr<IComponent> component = std::make_unique<T>(args...);
			component->Initialize(*this);
			components.emplace(type, std::move(component));

			Dispatch<T, Events::Initialization>();
			Logger::Get().Log(T::ClassName() + " initialized!");

			knownComponentTypes.insert(type);
		}

		template<Component T>
		T& GetComponent() const {
			return dynamic_cast<T&>(GetComponent(T::TypeId()));
		}

		template<Component T>
		T* TryGetComponent() const {
			return dynamic_cast<T*>(TryGetComponent(T::TypeId()));
		}

		void DestroyAll();

		template<Component T>
		bool HasComponent() const {
			return HasComponent(T::TypeId());
		}

		bool HasComponent(const uuids::uuid& compType) const;

	private:
		template<typename T>
		void CheckDependencies() {
			for (auto&& dependency : T::Dependencies()) {
				if (!HasComponent(dependency.type)) {
					SIS_THROW(AssembleString(
						ComponentRegistry::GetComponentName(dependency.type),
						" must be initialized before ",
						T::ClassName())
					);
				}
			}
		}

		template<Component T>
		void UpdateSubscriberLists() {
			UpdateSubscriberLists<T, Events::Initialization>();
		}

		template<Component T, ComponentEvent EventT>
		void UpdateSubscriberLists() {
			for (auto&& watched : T::WatchList(EventT{})) {
				subscriberLists[EventT::Id()][watched.type].push_back({ T::TypeId() });
			}
		}

		template<Component T, ComponentEvent EventT>
		void Dispatch() {
			for (auto subscriber : subscriberLists[EventT::Id()][T::TypeId()]) {
				auto comp = TryGetComponent(subscriber.type);
				if (comp != nullptr) {
					comp->HandleEvent(EventT{}, T::TypeId());
				}
			}
		}

		IComponent& GetComponent(const uuids::uuid& type) const;
		IComponent* TryGetComponent(const uuids::uuid& type) const;

		std::unordered_map<uuids::uuid, std::unique_ptr<IComponent>> components;

		using SubscriberLists = 
			std::unordered_map<
				uuids::uuid, // Event type
				std::unordered_map<
					uuids::uuid, // Component type
					ComponentReferences // Subscribers
				>
			>;
		SubscriberLists subscriberLists;
		DependencyGraph dependencyGraph;

		std::unordered_set<uuids::uuid> knownComponentTypes;
	};
}