#include "SmashCampusPlayerController.h"
#include "Engine/World.h"
#include "Engine/Engine.h"
#include "DrawDebugHelpers.h"
#include "Components/PrimitiveComponent.h"
#include "InputCoreTypes.h"

void ASmashCampusPlayerController::BeginPlay()
{
    Super::BeginPlay();
    bShowMouseCursor = true;
    bEnableTouchEvents = true;
    bEnableClickEvents = true;
}

void ASmashCampusPlayerController::SetupInputComponent()
{
    Super::SetupInputComponent();
    InputComponent->BindTouch(IE_Pressed, this, &ASmashCampusPlayerController::HandleTouch);
    InputComponent->BindKey(EKeys::LeftMouseButton, IE_Pressed, this, &ASmashCampusPlayerController::HandleClick);
}

void ASmashCampusPlayerController::HandleTouch(ETouchIndex::Type FingerIndex, FVector Location)
{
    AttackAtScreenPosition(FVector2D(Location.X, Location.Y));
}

void ASmashCampusPlayerController::HandleClick()
{
    float X = 0.0f, Y = 0.0f;
    if (GetMousePosition(X, Y)) AttackAtScreenPosition(FVector2D(X, Y));
}

void ASmashCampusPlayerController::AttackAtScreenPosition(const FVector2D& ScreenPosition)
{
    FVector WorldOrigin, WorldDirection;
    if (!DeprojectScreenPositionToWorld(ScreenPosition.X, ScreenPosition.Y, WorldOrigin, WorldDirection)) return;
    FHitResult Hit;
    FCollisionQueryParams Params(SCENE_QUERY_STAT(CampusAttack), true);
    if (!GetWorld()->LineTraceSingleByChannel(Hit, WorldOrigin, WorldOrigin + WorldDirection * 100000.0f, ECC_Visibility, Params)) return;

    UPrimitiveComponent* Component = Hit.GetComponent();
    if (Component && Component->ComponentHasTag(TEXT("Destructible")))
    {
        Component->SetCollisionProfileName(TEXT("PhysicsActor"));
        Component->SetSimulatePhysics(true);
        Component->AddImpulseAtLocation(WorldDirection * 220000.0f, Hit.ImpactPoint);
        Component->ComponentTags.Remove(TEXT("Destructible"));
    }
    DrawDebugLine(GetWorld(), WorldOrigin, Hit.ImpactPoint, FColor::Cyan, false, 0.15f, 0, 10.0f);
}
