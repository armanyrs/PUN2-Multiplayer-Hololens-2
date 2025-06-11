using UnityEngine;
// Jika Anda menggunakan versi MRTK yang lebih baru dan memerlukan namespace khusus untuk TrackedObjectType:
// using Microsoft.MixedReality.Toolkit.SDK.Tracking; 

public class RoboPetFollowController : MonoBehaviour
{
    [Tooltip("Target yang akan diikuti (biasanya Main Camera).")]
    public Transform userTarget;

    [Tooltip("Jarak minimal (horizontal) sebelum robo-pet mulai mengikuti pengguna.")]
    public float followDistanceThreshold = 2.5f;

    [Tooltip("Jarak minimal (horizontal) yang dijaga robo-pet dari pengguna saat mengikuti.")]
    public float minimumFollowDistance = 1.5f;

    [Tooltip("Kecepatan gerakan robo-pet.")]
    public float moveSpeed = 1.2f; // Sedikit ditingkatkan untuk responsivitas

    [Tooltip("Kecepatan rotasi robo-pet (untuk menghadap pengguna secara horizontal).")]
    public float rotationSpeed = 7.0f; // Ditingkatkan untuk rotasi yang lebih responsif

    private float groundLevelY; // Ketinggian Y di mana robo-pet akan beroperasi
    // private Microsoft.MixedReality.Toolkit.Utilities.Solvers.SolverHandler solverHandler; // Hanya jika Anda membutuhkannya untuk setup target

    void Start()
    {
        // Coba dapatkan userTarget (Main Camera) jika belum di-assign
        if (userTarget == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                userTarget = mainCamera.transform;
                Debug.Log("RoboPetAdvancedController: userTarget diatur ke Main Camera secara otomatis.");
            }
            else
            {
                Debug.LogError("RoboPetAdvancedController: userTarget (Main Camera) belum di-assign dan tidak dapat ditemukan! Skrip akan dinonaktifkan.");
                enabled = false;
                return;
            }
        }

        // Simpan ketinggian Y awal robo-pet sebagai 'groundLevelY'.
        // Ini mengasumsikan robo-pet muncul/ditempatkan pada ketinggian lantai yang benar.
        groundLevelY = transform.position.y;
        Debug.Log($"RoboPetAdvancedController: Inisialisasi berhasil. Ground Level Y ditetapkan ke: {groundLevelY}. Target: {userTarget.name}");

        // Anda bisa mendapatkan SolverHandler di sini jika memerlukannya untuk konfigurasi target awal,
        // tetapi pastikan solver di dalamnya yang mengatur posisi/rotasi sudah nonaktif.
        // solverHandler = GetComponent<Microsoft.MixedReality.Toolkit.Utilities.Solvers.SolverHandler>();
        // if (solverHandler != null)
        // {
        //    // Contoh: solverHandler.TrackedTargetType = Microsoft.MixedReality.Toolkit.SDK.Tracking.TrackedObjectType.Head;
        //    Debug.Log("SolverHandler ditemukan. Pastikan solver posisi/rotasi di dalamnya tidak konflik dengan skrip ini.");
        // }
    }

    void Update()
    {
        if (userTarget == null)
        {
            return; // Jangan lakukan apa-apa jika tidak ada target
        }

        // --- LOGIKA ROTASI HORIZONTAL (YAW ONLY) ---
        // Ambil arah dari robo-pet ke pengguna
        Vector3 directionToUser = userTarget.position - transform.position;
        directionToUser.y = 0; // Kunci: Abaikan perbedaan ketinggian untuk rotasi, jadi hanya berputar horizontal

        // Hanya lakukan rotasi jika ada arah yang valid (menghindari error LookRotation dengan zero vector)
        if (directionToUser.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToUser.normalized);
            // Gunakan Slerp untuk rotasi yang lebih halus
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // --- LOGIKA MENGIKUTI PENGGUNA SECARA HORIZONTAL ---
        // Hitung posisi horizontal robo-pet dan pengguna (menggunakan groundLevelY)
        Vector3 petPositionOnGround = new Vector3(transform.position.x, groundLevelY, transform.position.z);
        Vector3 userPositionOnGround = new Vector3(userTarget.position.x, groundLevelY, userTarget.position.z);

        float currentHorizontalDistance = Vector3.Distance(petPositionOnGround, userPositionOnGround);

        // Jika jarak lebih besar dari threshold, robo-pet bergerak mendekat
        if (currentHorizontalDistance > followDistanceThreshold)
        {
            Vector3 directionToMove = (userPositionOnGround - petPositionOnGround).normalized;
            // Target berhenti pada minimumFollowDistance dari pengguna di bidang horizontal
            Vector3 targetPosition = userPositionOnGround - directionToMove * minimumFollowDistance;

            // Pastikan targetPosition.y selalu sama dengan groundLevelY
            targetPosition.y = groundLevelY;

            // Gerakkan robo-pet ke posisi target menggunakan Lerp untuk kehalusan
            // Pastikan transform.position yang digunakan untuk Lerp juga dikoreksi Y-nya agar lerping tetap di ground plane
            Vector3 currentPosCorrectedY = new Vector3(transform.position.x, groundLevelY, transform.position.z);
            transform.position = Vector3.Lerp(currentPosCorrectedY, targetPosition, moveSpeed * Time.deltaTime);
        }

        // Koreksi akhir posisi Y jika ada deviasi kecil (misalnya karena fisika atau rounding errors)
        // Ini memastikan robo-pet tetap "menempel" di groundLevelY
        if (Mathf.Abs(transform.position.y - groundLevelY) > 0.01f) // Toleransi kecil untuk menghindari jitter
        {
            Vector3 correctedPosition = transform.position;
            correctedPosition.y = groundLevelY;
            transform.position = correctedPosition;
             Debug.LogWarning("RoboPetAdvancedController: Posisi Y dikoreksi kembali ke ground level.");
        }
    }
}