import os
import sys
from datetime import datetime
import requests as req
from tqdm import tqdm
import time
import zipfile

def download_file(url, fdir, fname):
    r = req.get(url, stream=True)
    size = int(r.headers.get('content-length', 0))
    
    size_in_mb = size / (1024 * 1024)
    print(f"[info] {fname} {size_in_mb: .2f}MB")
    print(f"[download] Destination: {os.getcwd()}/{fname}")
    
    start_time = time.time()
    with open(fname, 'wb') as file, tqdm(total=size, unit='B', unit_scale=True, desc="[download] Downloading file") as pbar:
        for data in r.iter_content(chunk_size=1024):
            file.write(data)
            pbar.update(len(data))
    
    download_time = time.time() - start_time
    format_time = time.strftime("%H:%M:%S", time.gmtime(download_time))
    download_mbps = size_in_mb / download_time
    print(f"[download] 100% of {size_in_mb: .2f}MB in {format_time} at {download_mbps:.2f}MB/s")

def extract_file(fname, fdir):
    with zipfile.ZipFile(fname, 'r') as ref:
        size = sum(len(ref.read(name)) for name in ref.namelist())
    
    size_in_mb = size / (1024 * 1024)
    print(f"[extract] Destination: {os.getcwd()}/{fdir}")
    
    start_time = time.time()
    with zipfile.ZipFile(fname, 'r') as ref:
        ref.extractall(fdir)
    
    extract_time = time.time() - start_time
    format_time = time.strftime("%H:%M:%S", time.gmtime(extract_time))
    extract_mbps = size_in_mb / extract_time
    print(f"[info] {os.getcwd()}/{fdir} {os.listdir(fdir)}")
    print(f"[extract] 100% of {size_in_mb: .2f}MB in {format_time} at {extract_mbps:.2f}MB/s")

def main():
    if len(sys.argv) < 2:
        sys.exit(1)
    
    url = sys.argv[1]
    fdir = os.path.join("download", datetime.now().strftime("%d%H%M%S"))
    fname = f"{fdir}.zip"
    if not os.path.exists(fdir):
        os.makedirs(fdir)
    
    download_file(url, fdir, fname)
    extract_file(fname, fdir)

if __name__ == "__main__":
    main()
