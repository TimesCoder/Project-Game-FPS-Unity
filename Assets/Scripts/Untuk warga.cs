void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player"))
    {
        FindObjectOfType<CitizenRescueMission>().OnCitizenSaved();
        Destroy(gameObject); // hapus warga dari scene
    }
}
