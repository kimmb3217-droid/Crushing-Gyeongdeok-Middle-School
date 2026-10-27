#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerController.h"
#include "SmashCampusPlayerController.generated.h"

UCLASS()
class SMASHCAMPUS_API ASmashCampusPlayerController : public APlayerController
{
    GENERATED_BODY()
protected:
    virtual void BeginPlay() override;
    virtual void SetupInputComponent() override;
private:
    void AttackAtScreenPosition(const FVector2D& ScreenPosition);
    void HandleTouch(ETouchIndex::Type FingerIndex, FVector Location);
    void HandleClick();
};
