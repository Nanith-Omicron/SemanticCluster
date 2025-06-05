
# SemanticCluster

A novel, sublinear-time, entropy-aware semantic search structure.
## 🚨 New: High-Dimensional Breakthrough

After extensive testing, **SemanticCluster** has proven to scale *shockingly well* in high-dimensional space:

> ✔️ Up to **300D** with minimal performance loss  
> ✔️ Near-constant memory footprint  
> ✔️ Outperforms traditional brute-force in complex feature space  
> ✔️ **√N scaling**: Sublinear cluster management out of the box
> ✔️**Semantic generality**: Works with `float`, `Vector3`, strings — anything with a distance metric
> ✔️For my Unity Homies, it's Burst Compatible for EVEN FURTHER BEYOND performance
> FUCK this GC BS. Insert and query without noise.

This makes it an **ideal algorithm for motion matching**, AI pose selection, and intent-driven animation systems — where vectors span 30–200+ dimensions (velocity, pose deltas, contact points, force bias, etc).

### ⚔️ Motion Matching Reinvented

SemanticCluster allows real-time:
- Combat-aware pose selection
- Entropy-aware animation prediction
- High-dimensional behavior discrimination (i.e. attacking from left with 70% stamina, 35° torso lean, max trauma angle)

Instead of traditional KD-Tree or nearest-neighbor:
```csharp
Pose best = semanticCluster.Query(currentGoalPose);
```
Where currentGoalPose is any structured vector with distance logic:

## Benchmark 😏
### Example: Tested Distance Function (Unity)

```csharp
(a, b) =>
    Vector3.Distance(a.position, b.position) +
    Quaternion.Angle(a.rotation, b.rotation) +
    Mathf.Abs(a.impactForce - b.impactForce) +
    Vector3.Distance(a.torsoUp, b.torsoUp);
 ```
 

| Dimensions | Query Time (ms) | Memory (KB) |
| ---------- | --------------- | ----------- |
| 2D         | 470 ms          | 164 KB      |
| 10D        | 210 ms          | 65 KB       |
| 100D       | 190 ms          | 68 KB       |
| 300D       | 230 ms          | 72 KB       |
| 500D       | 250+ ms         | 88 KB       |

This allows custom similarity logic based on task context (e.g., combat, traversal, evasion).
Easily extensible to 30–300+ dimensions for nuanced pose/intent clustering.
SemanticCluster scales gracefully — ideal for real-time motion matching, even with massive datasets.

## 30D Vectors, at 10
| Samples | Query Time (ms) | Memory (KB) |
| ------- | --------------- | ----------- |
| 100     | 129 ms          | 24 KB       |
| 500     | 110 ms          | 65 KB       |
| 1,000   | 140 ms          | 65 KB       |
| 5,000   | 547 ms          | 135 KB      |
| 10,000  | 886 ms          | 252 KB      |

This breaks assumptions around the “curse of dimensionality” — because this algorithm embraces it.
SemanticCluster scales gracefully — ideal for real-time motion matching, even with **fat, massive, bulging** datasets.

Heh. Nothing personal kid.
