#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "SmashCampusGameMode.generated.h"

UCLASS()
class SMASHCAMPUS_API ASmashCampusGameMode : public AGameModeBase
{
    GENERATED_BODY()
public:
    ASmashCampusGameMode();
    virtual void BeginPlay() override;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Campus")
    int32 TerrainResolution = 64;

    UPROPERTY(EditAnywhere, BlueprintReadWrite, Category="Campus")
    float TerrainSize = 12000.0f;

private:
    void BuildTerrain();
    void BuildCampus();
    void BuildBuilding(const FVector& Center, const FVector& Size, int32 Floors, FLinearColor FacadeColor);
    void AddBox(const FVector& Location, const FVector& Scale, FLinearColor Color, bool bDestructible = false);
    void AddTree(const FVector& Location, float Scale);
    UMaterialInterface* MakeMaterial(FLinearColor Color);
};
