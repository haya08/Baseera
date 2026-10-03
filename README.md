# Baseera

**Baseera** is an AI-powered marketing intelligence platform designed to help businesses understand their market, competitors, and brand-related information through structured knowledge and AI-powered analysis.

> 🚧 **Project Status:** Baseera is currently under development as a graduation project.

## 🎯 Overview

Marketing teams often need to collect information from multiple sources, understand how different entities are related, and continuously analyze market and competitor activity.

Baseera aims to provide a centralized system that transforms unstructured information into structured knowledge that can later be used for marketing analysis, insights, and recommendations.

The backend is built with **ASP.NET Core** and follows **Clean Architecture**, with AI-powered knowledge processing and a **Neo4j knowledge graph** at its core.

## ✨ Current Features

The current implementation includes:

* LLM-powered knowledge extraction
* Entity resolution
* Entity and alias management
* Knowledge persistence
* Neo4j knowledge graph
* Entity relationships
* Text embeddings
* Vector similarity search
* Full-text search
* Unit testing
* Integration testing

## 🧠 Knowledge Processing Pipeline

One of the core parts of Baseera is its knowledge processing pipeline.

The system processes unstructured information and transforms it into structured knowledge through several stages:

```text
Raw Information
      ↓
Knowledge Extraction
      ↓
Entity Resolution
      ↓
Knowledge Persistence
      ↓
Neo4j Knowledge Graph
```

### 1. Knowledge Extraction

Baseera uses an LLM to extract structured knowledge from unstructured text.

The extraction process identifies entities such as:

* Brands
* Products
* Services
* Features
* Topics
* People
* Campaigns
* Events
* Aliases

The extracted entities are then passed to the resolution stage.

### 2. Entity Resolution

Extracted entities may refer to entities that already exist in the knowledge graph.

Baseera resolves these entities using multiple strategies, including:

* Exact matching
* Alias matching
* Candidate generation
* LLM-based resolution judgment
* Ambiguity detection

The resolution process can determine whether an extracted entity:

* Matches an existing entity
* Matches an existing alias
* Should create a new entity
* Is ambiguous and requires further handling

### 3. Knowledge Persistence

After resolution, the resulting knowledge is persisted into the knowledge graph.

This includes:

* Entities
* Aliases
* Relationships
* Entity metadata

## 🕸️ Knowledge Graph

Baseera uses **Neo4j** to represent structured knowledge as a graph.

For example:

```text
        ┌─────────────┐
        │    Brand    │
        │    Nike     │
        └──────┬──────┘
               │
          HAS_PRODUCT
               │
               ▼
        ┌─────────────┐
        │   Product   │
        │   Air Max   │
        └─────────────┘
```

The graph allows relationships between entities to be represented explicitly and provides a foundation for future knowledge-based marketing analysis.

### Entity Types

The current knowledge model supports:

* Brand
* Product
* Service
* Feature
* Topic
* Person
* Campaign
* Event
* Alias

### Relationship Types

The current relationship model includes:

* `HasAlias`
* `HasProduct`
* `HasService`
* `HasFeature`
* `HasTopic`
* `RunsCampaign`
* `HasEvent`
* `RelatedTo`
* `LedBy`

## 🔎 Search & Entity Matching

Baseera combines multiple search techniques to improve entity matching.

### Full-Text Search

Neo4j full-text indexes are used to find relevant candidate entities based on textual information.

### Vector Search

Entities can also be represented using text embeddings.

The embeddings are stored in Neo4j and used for vector similarity search to retrieve semantically similar candidates.

This allows the system to identify entities that may be related even when their textual representations are not exact matches.

## 🤖 AI Integration

Baseera integrates Google's Gemini models for AI-powered processing.

The LLM is currently used for tasks such as:

* Knowledge extraction
* Entity resolution judgment

Embeddings are also generated for knowledge entities to support semantic similarity search.

## 🏗️ Architecture

The backend follows **Clean Architecture** to separate business logic, application services, infrastructure concerns, and the API layer.

The main layers are organized as:

```text
Baseera
│
├── Baseera.Domain
├── Baseera.Core
├── Baseera.Service
├── Baseera.Infrastructure
├── Baseera.Api
└── Baseera.Shared
```

The architecture is designed to keep the core application logic independent from external infrastructure and service implementations.

## 🧪 Testing

Baseera includes automated tests for important parts of the backend.

Current testing includes:

* Unit tests
* Integration tests
* Entity resolution scenarios
* Knowledge processing workflows

The entity resolution tests cover scenarios such as:

* Exact matches
* Alias matches
* New entities
* Ambiguous candidates
* LLM-based resolution decisions

## 🛠️ Tech Stack

### Backend

* **C#**
* **ASP.NET Core**
* **.NET**
* **Clean Architecture**

### AI & Data Processing

* **Google Gemini**
* **LLM-based Knowledge Extraction**
* **Entity Resolution**
* **Text Embeddings**

### Database & Search

* **Neo4j**
* **Knowledge Graphs**
* **Vector Search**
* **Full-Text Search**

### Testing & Tools

* **Unit Testing**
* **Integration Testing**
* **Git**
* **GitHub**

## 🚧 Planned Features

Baseera is still under active development.

Planned parts of the platform include:

* Social media data connectors
* Automated data collection
* Continuous knowledge updates
* Marketing analytics
* Competitor analysis
* AI-powered marketing insights
* AI-generated marketing recommendations

These features are part of the ongoing development roadmap and may evolve as the project progresses.

## 📌 Project Status

Baseera is an ongoing graduation project.

The core knowledge-processing foundation has been implemented, including knowledge extraction, entity resolution, persistence, and graph-based knowledge representation.

Further data connectors, analytics capabilities, and marketing intelligence features are currently being developed.

## 👩‍💻 Author

**Haya Ahmed Hussien**

* GitHub: https://github.com/haya08
* LinkedIn: https://linkedin.com/in/hayaahmedhussien
