# Use to convert .mov file into fMP4

.\ffmpeg.exe -i .\sample_full_hd_30_fps.mov -c:v libx264 -profile:v main -level 4.0 -pix_fmt yuv420p -c:a aac -b:a 128k -f segment -segment_time 2 -movflags frag_keyframe+empty_moov+default_base_moof out_%03d.mp4
