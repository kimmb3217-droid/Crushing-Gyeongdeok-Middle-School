#include "SmashCampusGameMode.h"
#include "SmashCampusPlayerController.h"
#include "ProceduralMeshComponent.h"
#include "Camera/CameraActor.h"
#include "Kismet/GameplayStatics.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "UObject/ConstructorHelpers.h"
#include "Engine/World.h"

ASmashCampusGameMode::ASmashCampusGameMode()
{
    PlayerControllerClass = ASmashCampusPlayerController::StaticClass();
    DefaultPawnClass = nullptr;
}

void ASmashCampusGameMode::BeginPlay()
{
    Super::BeginPlay();
    BuildTerrain();
    BuildCampus();

    const FVector CameraLocation(15500.0f, -20500.0f, 15000.0f);
    const FRotator CameraRotation = (FVector::ZeroVector - CameraLocation).Rotation();
    FActorSpawnParameters Params;
    ACameraActor* ViewCamera = GetWorld()->SpawnActor<ACameraActor>(ACameraActor::StaticClass(), CameraLocation, CameraRotation, Params);
    if (ViewCamera)
    {
        APlayerController* PC = UGameplayStatics::GetPlayerController(this, 0);
        if (PC) PC->SetViewTargetWithBlend(ViewCamera, 0.0f);
    }
}

UMaterialInterface* ASmashCampusGameMode::MakeMaterial(FLinearColor Color)
{
    UMaterialInstanceDynamic* Mat = UMaterialInstanceDynamic::Create(UMaterial::GetDefaultMaterial(MD_Surface), this);
    if (Mat) Mat->SetVectorParameterValue(TEXT("BaseColor"), Color);
    return Mat ? Mat : UMaterial::GetDefaultMaterial(MD_Surface);
}

void ASmashCampusGameMode::BuildTerrain()
{
    UProceduralMeshComponent* Terrain = NewObject<UProceduralMeshComponent>(this, TEXT("LandscapeTerrain"));
    SetRootComponent(Terrain);
    Terrain->RegisterComponent();
    Terrain->bUseAsyncCooking = true;

    const int32 N = FMath::Clamp(TerrainResolution, 16, 128);
    const float Half = TerrainSize * 0.5f;
    TArray<FVector> Vertices;
    TArray<int32> Triangles;
    TArray<FVector> Normals;
    TArray<FVector2D> UVs;
    TArray<FLinearColor> Colors;
    TArray<FProcMeshTangent> Tangents;
    Vertices.Reserve((N + 1) * (N + 1));
    for (int32 Y = 0; Y <= N; ++Y)
    {
        for (int32 X = 0; X <= N; ++X)
        {
            const float PX = -Half + TerrainSize * X / N;
            const float PY = -Half + TerrainSize * Y / N;
            const float Z = 70.0f * FMath::Sin(PX * 0.0007f) * FMath::Cos(PY * 0.0006f)
                + 28.0f * FMath::Sin((PX + PY) * 0.0013f);
            Vertices.Add(FVector(PX, PY, Z));
            UVs.Add(FVector2D((float)X / N * 12.0f, (float)Y / N * 12.0f));
            Colors.Add(FLinearColor(0.22f, 0.36f, 0.19f, 1.0f));
        }
    }
    for (int32 Y = 0; Y < N; ++Y)
    {
        for (int32 X = 0; X < N; ++X)
        {
            const int32 A = Y * (N + 1) + X;
            const int32 B = A + 1;
            const int32 C = A + N + 1;
            const int32 D = C + 1;
            Triangles.Append({A, C, B, B, C, D});
        }
    }
    for (int32 I = 0; I < Vertices.Num(); ++I)
    {
        Normals.Add(FVector::UpVector);
        Tangents.Add(FProcMeshTangent(1.0f, 0.0f, 0.0f));
    }
    Terrain->CreateMeshSection_LinearColor(0, Vertices, Triangles, Normals, UVs, Colors, Tangents, true);
    Terrain->SetMaterial(0, MakeMaterial(FLinearColor(0.24f, 0.39f, 0.20f)));
}

void ASmashCampusGameMode::AddBox(const FVector& Location, const FVector& Scale, FLinearColor Color, bool bDestructible)
{
    UStaticMeshComponent* Mesh = NewObject<UStaticMeshComponent>(this);
    static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeMesh(TEXT("/Engine/BasicShapes/Cube.Cube"));
    if (CubeMesh.Succeeded()) Mesh->SetStaticMesh(CubeMesh.Object);
    Mesh->SetWorldLocation(Location);
    Mesh->SetWorldScale3D(Scale / 100.0f);
    Mesh->SetMaterial(0, MakeMaterial(Color));
    Mesh->SetCollisionProfileName(TEXT("BlockAll"));
    Mesh->RegisterComponent();
    if (bDestructible)
    {
        Mesh->SetSimulatePhysics(false);
        Mesh->SetNotifyRigidBodyCollision(true);
        Mesh->SetGenerateOverlapEvents(true);
        Mesh->ComponentTags.Add(TEXT("Destructible"));
    }
}

void ASmashCampusGameMode::BuildBuilding(const FVector& Center, const FVector& Size, int32 Floors, FLinearColor FacadeColor)
{
    const float FloorHeight = Size.Z / FMath::Max(1, Floors);
    AddBox(Center, Size, FacadeColor, true);
    AddBox(Center + FVector(0, 0, Size.Z * 0.5f + 35.0f), FVector(Size.X + 90.0f, Size.Y + 90.0f, 70.0f), FLinearColor(0.12f, 0.14f, 0.16f), true);

    const int32 WindowRows = FMath::Max(1, Floors);
    const int32 WindowCols = FMath::Max(3, FMath::FloorToInt(Size.X / 190.0f));
    for (int32 Row = 0; Row < WindowRows; ++Row)
    {
        const float Z = Center.Z - Size.Z * 0.5f + FloorHeight * (Row + 0.55f);
        for (int32 Col = 0; Col < WindowCols; ++Col)
        {
            const float X = Center.X - Size.X * 0.5f + Size.X * (Col + 0.5f) / WindowCols;
            AddBox(FVector(X, Center.Y - Size.Y * 0.5f - 7.0f, Z),
                FVector(82.0f, 18.0f, 95.0f), FLinearColor(0.08f, 0.24f, 0.31f), true);
        }
    }
    AddBox(Center + FVector(0, -Size.Y * 0.5f - 24.0f, -Size.Z * 0.5f + 105.0f),
        FVector(180.0f, 45.0f, 210.0f), FLinearColor(0.16f, 0.20f, 0.22f), true);
}

void ASmashCampusGameMode::AddTree(const FVector& Location, float Scale)
{
    AddBox(Location + FVector(0, 0, 120.0f * Scale), FVector(35.0f, 35.0f, 240.0f) * Scale, FLinearColor(0.27f, 0.16f, 0.08f));
    UStaticMeshComponent* Crown = NewObject<UStaticMeshComponent>(this);
    static ConstructorHelpers::FObjectFinder<UStaticMesh> SphereMesh(TEXT("/Engine/BasicShapes/Sphere.Sphere"));
    if (SphereMesh.Succeeded()) Crown->SetStaticMesh(SphereMesh.Object);
    Crown->SetWorldLocation(Location + FVector(0, 0, 300.0f * Scale));
    Crown->SetWorldScale3D(FVector(1.9f, 1.9f, 2.1f) * Scale);
    Crown->SetMaterial(0, MakeMaterial(FLinearColor(0.10f, 0.28f, 0.10f)));
    Crown->SetCollisionEnabled(ECollisionEnabled::NoCollision);
    Crown->RegisterComponent();
}

void ASmashCampusGameMode::BuildCampus()
{
    AddBox(FVector(0, -2400, 15), FVector(11800, 720, 35), FLinearColor(0.13f, 0.14f, 0.15f));
    AddBox(FVector(0, 0, 22), FVector(6500, 2800, 45), FLinearColor(0.29f, 0.38f, 0.22f));
    AddBox(FVector(0, -1450, 28), FVector(5200, 180, 25), FLinearColor(0.72f, 0.68f, 0.54f));

    BuildBuilding(FVector(0, 400, 520), FVector(1700, 650, 1000), 4, FLinearColor(0.68f, 0.57f, 0.43f));
    BuildBuilding(FVector(-1350, 1050, 350), FVector(850, 600, 660), 3, FLinearColor(0.76f, 0.72f, 0.61f));
    BuildBuilding(FVector(1380, 950, 300), FVector(900, 700, 560), 3, FLinearColor(0.62f, 0.67f, 0.65f));
    BuildBuilding(FVector(-900, -850, 260), FVector(1050, 520, 480), 2, FLinearColor(0.73f, 0.64f, 0.51f));
    BuildBuilding(FVector(1000, -800, 330), FVector(900, 560, 620), 3, FLinearColor(0.65f, 0.60f, 0.53f));

    for (int32 I = 0; I < 11; ++I)
    {
        const float X = -4600.0f + I * 920.0f;
        AddTree(FVector(X, -3100.0f, 20.0f), 0.85f + (I % 3) * 0.12f);
        AddTree(FVector(X, 2450.0f, 20.0f), 0.9f + (I % 2) * 0.18f);
    }
    for (int32 I = 0; I < 7; ++I)
    {
        AddTree(FVector(-2800.0f, -900.0f + I * 520.0f, 20.0f), 0.8f);
        AddTree(FVector(2900.0f, -900.0f + I * 520.0f, 20.0f), 0.9f);
    }
}
