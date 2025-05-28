# GroupFinder

> **Public codebase. Not open source.**
> This repository exists to showcase backend craftsmanship, not to encourage contributions.

---

## ✨ What is GroupFinder?

**GroupFinder** is a social matchmaking engine for World of Warcraft guilds, rosters, and players who want **more than a "/who paladin" search bar**.

It exists to fix a deep, structural pain:

* Recruitment is opaque, toxic, fragmented
* Solo players often give up before finding a group
* Guilds lose weeks chasing unreliable trials

GroupFinder aims to solve this through:

* **OAuth-based login via Battle.net**
* **Swipe-based UI** for clarity, not speed
* **Profiles based on intent, not just logs**
* **Matching engine** focused on fit, reliability, and transparency

---

## 📄 Strategic Goals

This repository showcases:

* Domain-driven design applied to gaming social networks
* Clean Architecture + CQRS
* Test-first development
* Modular structure with Battle.net integration

It is **not**:

* A commercial SaaS (yet)
* A collaborative project
* A finished MVP

---

## 📁 Folder Structure

```bash
/GroupFinder
├── GroupFinder.Api            # REST API entrypoint
├── GroupFinder.Application    # Use cases, CQRS handlers
├── GroupFinder.Domain         # Aggregates, VOs, core logic
├── GroupFinder.Infrastructure # External integrations (incl. BattleNet)
├── GroupFinder.Persistence    # EF Core, Migrations
├── GroupFinder.Presentation   # Formatting, Resources, CLI/Views
├── GroupFinder.Tests          # xUnit test suite
```

---

## 🌊 Licensing

**Business Source License 1.1 (BUSL-1.1)**

This project is released under the Business Source License:

> You may read, clone, and explore the code.
> You may **not** use it in production or in any commercial product without explicit permission.
> License will convert to Apache 2.0 on **January 1st, 2029**.

For more details, see [LICENSE.md](LICENSE.md).

---

## 🚀 Why it's public

Because **too many devs talk craft, not enough show it**.

This repo is:

* A sandbox for architectural rigor
* A long-term portfolio asset
* A technical narrative in progress

You're welcome to explore, clone, and learn from it.
But it’s **not open for contributions.**

Built by a (former) GM who got tired of guild drama and decided to write the tool he wished existed.
